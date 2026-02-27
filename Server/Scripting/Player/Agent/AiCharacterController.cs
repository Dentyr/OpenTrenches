using OpenTrenches.Common.Contracts.DTO;
using OpenTrenches.Common.Contracts.DTO.PlayerCommands;
using OpenTrenches.Server.Scripting.Player;

namespace OpenTrenches.Server.Scripting.Player.Agent;

public interface AiCharacterController
{
    Character Character { get; }

    /// <summary>
    /// Called on each tick, thinks about what the character should do next
    /// </summary>
    public void Think()
    {
        
    }
}
