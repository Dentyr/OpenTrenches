using Godot;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.World;
using OpenTrenches.Server.Scripting.World;

namespace OpenTrenches.Server.Scripting.Player.Agent;

/// <summary>
/// Shoots at the closest enemy character.
/// </summary>
public class HoldTask : AbstractAgentTask
{
    /// <summary>
    /// Moves to secure the area around this position and seeks a good place to entrench
    /// </summary>
    private readonly Vector2 _area;

    /// <summary>
    /// The specific position to secure within the area
    /// </summary>
    private Vector2 _targetPosition;

    /// <summary>
    /// Marker to check if the character is in a stable location
    /// </summary>
    private bool _positioned;

    private IWorldObject? _currentTarget;

    public HoldTask(Vector2 position, IWorldObject? Target = null)
    {
        _area = position;
        _targetPosition = position;
        _currentTarget = Target;
    }

    /// <remarks>
    /// Hold tasks are held indefinitely until changed from above
    /// </remarks>
    public override bool Reason(Character character, IWorld2DQueryService queryService, IServerChunkArray chunks)
    {
        _positioned = TaskServices.Navigate(
            character, _targetPosition, queryService, 
            error: 10
        );

        _currentTarget = TaskServices.FindTarget(character, queryService);

        return false;
    }

    public override void Process(Character character, IWorld2DQueryService queryService, IServerChunkArray chunks)
    {
        // If not yet positioned, keep stepping until they are.
        if (!_positioned)
        {
            if (TaskServices.Step(character, _targetPosition, chunks, 
                error: 1)
            ) {            
                _positioned = true;
            }
        }
        
        if (TaskServices.EnemyValid(character, _currentTarget, 20)) 
        {
            character.Direction = _currentTarget.Position;
            TaskServices.ReasonAttack(character);
        }
        else
        {
            _currentTarget = null;
            character.TryClear(CharacterState.Shooting);
        }
    }
}
