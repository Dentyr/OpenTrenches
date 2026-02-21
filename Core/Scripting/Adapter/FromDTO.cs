using OpenTrenches.Common.Contracts.DTO.DataModel;
using OpenTrenches.Common.Factions;
using OpenTrenches.Core.Scripting.Player;
using OpenTrenches.Core.Scripting.Teams;

namespace OpenTrenches.Core.Scripting.Adapter;

public static class FromDTO
{
    public static Character Convert(CharacterDTO dTO, IClientState clientState)
    {
        return new Character(dTO.ID, dTO.Team, clientState, dTO.Position, dTO.Health);
    }
    public static ClientTeam Convert(TeamDTO dTO)
    {
        return new ClientTeam(dTO.ID, FactionRecordLibary.StandardFaction);
    }
}