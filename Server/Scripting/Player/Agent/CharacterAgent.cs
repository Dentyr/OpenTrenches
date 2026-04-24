using Godot;
using OpenTrenches.Common.World;

namespace OpenTrenches.Server.Scripting.Player.Agent;

/// <summary>
/// makes NPC character decisions
/// </summary>
public class CharacterAgent
{
    private float _variable = GD.Randf() - 0.5f;

    private WorldPosition _movementTarget = new WorldPosition(Vector2.Zero);

    private IWorldObject? _fireTarget;

    private AbstractAgentTask _task;

    public CharacterAgent()
    {
        _task = new IdleTask();
    }

    /// <summary>
    /// Called on each tick, thinks about what the character should do next
    /// </summary>
    public void Think(Character character, ICharacterAdapter adapter)
    {


        if (GD.Randf() > 0.975f)
        {
            _task = _task.Reason(character, adapter);
            DecideMovementTarget(character, adapter);
            // DecideShootingTarget(character, adapter);
        }

        if (character.Position.DistanceSquaredTo(_movementTarget.Position) > 0.3f) 
            character.MoveIn(_movementTarget.Position - character.Position);
        else 
            character.MoveIn(Vector2.Zero);

        _task.Process(character, adapter);

    }

    private void DecideMovementTarget(Character character, ICharacterAdapter adapter)
    {
        //     character.MoveIn(_movementTarget.Position - character.Position);
        // }
        var movement = character.Team.ID switch
        {
            0 => Vector2.Right.Rotated(_variable),
            1 => Vector2.Left.Rotated(_variable),
            _ => Vector2.Zero,
        };

        _movementTarget = new WorldPosition(character.Position + (movement * 50f));
    }
}
