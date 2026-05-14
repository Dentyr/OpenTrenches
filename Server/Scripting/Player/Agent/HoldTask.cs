using System;
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
    private const float DefaultRange = 5f;

    /// <summary>
    /// Moves to secure the area around this position and seeks a good place to entrench
    /// </summary>
    private readonly Vector2 _area;
    /// <summary>
    /// Radius around area to secure
    /// </summary>
    private readonly float _range;

    /// <summary>
    /// The specific position to secure within the area
    /// </summary>
    private Vector2 _targetPosition;

    /// <summary>
    /// Marker to check if the character is in a stable location
    /// </summary>
    private bool _positioned;

    private IWorldObject? _currentTarget;

    /// <summary>
    /// Creates a task to hold <paramref name="range"/> around <paramref name="position"/>
    /// </summary>
    public HoldTask(Vector2 position, float range = DefaultRange)
    {
        _area = position;
        _range = range;

        _targetPosition = _area + new Vector2((GD.Randf() - 0.5f) * 2 * range, (GD.Randf() - 0.5f) * 2 * range);
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
