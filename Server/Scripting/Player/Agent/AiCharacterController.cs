using Godot;
using OpenTrenches.Server.Scripting.Player;

namespace OpenTrenches.Server.Scripting.Player.Agent;

public class AiCharacterController
{
    private Character Character { get; }

    private ServerState ServerState { get; }

    private float variable = (GD.Randf() - 0.5f);

    public AiCharacterController(ServerState ServerState)
    {
        this.ServerState = ServerState;
        Character = ServerState.CreateCharacter();
    }

    /// <summary>
    /// Called on each tick, thinks about what the character should do next
    /// </summary>
    public void Think()
    {
        var movement = Character.Team.ID switch
        {
            0 => Vector3.Right.Rotated(Vector3.Up, variable),
            1 => Vector3.Left.Rotated(Vector3.Up, variable),
            _ => Vector3.Zero,
        };

        Character.MoveIn(movement);
    }
}
