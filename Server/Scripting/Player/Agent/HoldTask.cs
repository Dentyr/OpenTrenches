using System;
using Godot;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.World;
using OpenTrenches.Core.Scripting.World;
using OpenTrenches.Server.Scripting.World;

namespace OpenTrenches.Server.Scripting.Player.Agent;

/// <summary>
/// Shoots at the closest enemy character.
/// </summary>
public class HoldTask : AbstractAgentTask
{
    /// <summary>
    /// Chance per reason tick to find another random place to secure
    /// </summary>
    private const float ChangeToSeekNewSecureLocation = 0.1f;

    private const float DefaultRange = 5f;
    /// <summary>
    /// When the agent has determined a location to secure, look this number of cells away to find the ideal position
    /// </summary>
    private const int SeekPositionRange = 2;

    /// <summary>
    /// Moves to secure the area around this position and seeks a good place to entrench
    /// </summary>
    public readonly Vector2 TargetArea;
    /// <summary>
    /// Radius around area to secure
    /// </summary>
    private readonly float _range;

    /// <summary>
    /// The specific position to secure within the area
    /// </summary>
    private Vector2I _secureTarget;

    /// <summary>
    /// The exact position to move to
    /// </summary>
    private Vector2 _targetPosition;

    /// <summary>
    /// Marker to check if the character is in a stable location
    /// </summary>
    public bool Positioned { get; private set; }

    private IWorldObject? _currentTarget;

    /// <summary>
    /// Creates a task to hold <paramref name="range"/> around <paramref name="position"/>
    /// </summary>
    public HoldTask(Vector2 position, float range = DefaultRange)
    {
        TargetArea = position;
        _range = range;

        _secureTarget = SeekSecureTarget();
        _targetPosition = _secureTarget.CellToPosition();
    }

    /// <summary>
    /// Looks for a random securable target location
    /// </summary>
    private Vector2I SeekSecureTarget()
    {
        float angle = GD.Randf() * Mathf.Tau;
        float radius = Mathf.Sqrt(GD.Randf()) * _range;
        return (Vector2I)(TargetArea + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius);
    }

    /// <summary>
    /// Searches for the exact position to move to in the target location, looking for the nearest entrenched position
    /// </summary>
    /// <returns></returns>
    private Vector2 SeekPositionTarget(IWorld2DQueryService queryService, IServerChunkArray chunks)
    {
        Vector2I best = _secureTarget;
        // score for the current best position. lower is better
        float bestScore = SeekPositionRange * SeekPositionRange + 10;

        int startX = Math.Max(0, _secureTarget.X - SeekPositionRange);
        int endX = Math.Min(chunks.SizeX, _secureTarget.X + SeekPositionRange + 1);

        int startY = Math.Max(0, _secureTarget.Y - SeekPositionRange);
        int endY = Math.Min(chunks.SizeY, _secureTarget.Y + SeekPositionRange + 1);

        for (int x = startX; x < endX; x ++)
        {
            for (int y = startY; y < endY; y ++)
            {
                if (chunks[x, y] == TileType.Trench)
                {
                    Vector2I cell = new(x, y);
                    float score = _secureTarget.DistanceSquaredTo(cell);
                    if (score < bestScore)
                    {
                        bestScore = score;
                        best = cell;
                    }
                }
            }
        }
        return best.CellToPosition();
    }



    /// <remarks>
    /// Hold tasks are held indefinitely until changed from above
    /// </remarks>
    public override bool Reason(Character character, IWorld2DQueryService queryService, IServerChunkArray chunks)
    {
        if (GD.Randf() < ChangeToSeekNewSecureLocation)
            _secureTarget = SeekSecureTarget();
        _targetPosition = SeekPositionTarget(queryService, chunks);
        Positioned = TaskServices.Navigate(
            character, _targetPosition, queryService, 
            error: 1
        );

        _currentTarget = TaskServices.FindTarget(character, queryService);

        return false;
    }

    public override void Process(Character character, IWorld2DQueryService queryService, IServerChunkArray chunks)
    {
        // If not yet positioned, keep stepping until they are.
        if (!Positioned)
        {
            if (TaskServices.Step(character, _targetPosition, chunks, 
                error: 1)
            ) {
                Positioned = true;
            }
        }
        
        if (TaskServices.EnemyValid(character, _currentTarget, 20)) 
        {
            TaskServices.ReasonAttack(character, _currentTarget.Position);
        }
        else
        {
            _currentTarget = null;
            character.CancelAttack();
        }
    }
}
