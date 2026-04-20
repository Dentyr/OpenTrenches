using Godot;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.World;

namespace OpenTrenches.Server.Scripting.Player.Agent;

public class CharacterAgent
{
    private float _variable = GD.Randf() - 0.5f;

    private IWorldObject _movementTarget = new WorldPosition(Vector2.Zero);

    private IWorldObject? _fireTarget;

    public CharacterAgent()
    {
    }

    /// <summary>
    /// Called on each tick, thinks about what the character should do next
    /// </summary>
    public void Think(Character character, ICharacterAdapter adapter)
    {


        if (GD.Randf() > 0.975f)
        {
            DecideMovementTarget(character, adapter);
            DecideShootingTarget(character, adapter);
        }

        if (character.Position.DistanceSquaredTo(_movementTarget.Position) > 0.3f) 
            character.MoveIn(_movementTarget.Position - character.Position);
        else 
            character.MoveIn(Vector2.Zero);

        if (_fireTarget is not null) 
            character.Direction = _fireTarget.Position;

        if (character.State == CharacterState.Shooting &&
            (
                _fireTarget is null ||
                _fireTarget.Hp <= 0 ||
                _fireTarget.Position.DistanceSquaredTo(character.Position) > 400
            )
        )
            character.TrySwitch(CharacterState.Idle);

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
    private void DecideShootingTarget(Character character, ICharacterAdapter adapter)
    {        
        if (adapter.Query(new WorldQuery.Threats()) is WorldQueryResult.Found found)
        {
            foreach (Character target in found.Characters)
            {
                //TODO swap to random target
                if (target.Team != character.Team)
                {
                    _fireTarget = target;
                    character.TrySwitch(Common.Contracts.Defines.CharacterState.Shooting);
                    return;
                }
            }
        }



    }
}
