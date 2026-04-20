using Godot;
using OpenTrenches.Server.Scripting.Player;

namespace OpenTrenches.Server.Scripting.Player.Agent;

public class CharacterAgent
{
    private float variable = GD.Randf() - 0.5f;

    public CharacterAgent()
    {
    }

    /// <summary>
    /// Called on each tick, thinks about what the character should do next
    /// </summary>
    public void Think(Character character, ICharacterAdapter adapter)
    {
        var movement = character.Team.ID switch
        {
            0 => Vector2.Right.Rotated(variable),
            1 => Vector2.Left.Rotated(variable),
            _ => Vector2.Zero,
        };

        character.MoveIn(movement);
    }
}
