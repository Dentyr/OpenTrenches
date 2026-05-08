using System;
using Godot;
using OpenTrenches.Common.World;
using OpenTrenches.Server.Scene.World;

namespace OpenTrenches.Server.Scripting.Player.Agent;

/// <summary>
/// makes NPC character decisions
/// </summary>
public class CharacterAgent
{
    private float _variable = GD.Randf() - 0.5f;

    private readonly Character _character;

    private WorldPosition _movementTarget = new WorldPosition(Vector2.Zero);

    private IWorldObject? _fireTarget;

    private AbstractAgentTask _task;

    private int _planCounter = Random.Shared.Next(1000);

    public CharacterAgent(Character character)
    {
        _character = character;
        _task = new IdleTask();
    }

    /// <summary>
    /// Called on each tick, thinks about what the character should do next
    /// </summary>
    public void Think(World2DQueryService queryService)
    {
        _planCounter --;
        if (_planCounter <= 0)
            Plan(queryService);

        if (GD.Randf() > 0.975f)
        {
            _task = _task.Reason(_character, queryService);
            // DecideMovementTarget(character, adapter);
        }

        // if (character.Position.DistanceSquaredTo(_movementTarget.Position) > 0.3f) 
        //     character.MoveIn(_movementTarget.Position - character.Position);
        // else 
        //     character.MoveIn(Vector2.Zero);

        _task.Process(_character, queryService);
    }

    public void Plan(World2DQueryService adapter)
    {
        
    }

    // private void DecideMovementTarget(Character character, ICharacterAdapter adapter)
    // {
    //     //     character.MoveIn(_movementTarget.Position - character.Position);
    //     // }
    //     var movement = character.Team.ID switch
    //     {
    //         0 => Vector2.Right.Rotated(_variable),
    //         1 => Vector2.Left.Rotated(_variable),
    //         _ => Vector2.Zero,
    //     };

    //     _movementTarget = new WorldPosition(character.Position + (movement * 50f));
    // }
}