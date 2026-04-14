using System;
using OpenTrenches.Common.Contracts.DTO.DataModel;
using OpenTrenches.Common.Factions;
using OpenTrenches.Common.World;
using OpenTrenches.Core.Scripting.Player;
using OpenTrenches.Core.Scripting.Teams;
using OpenTrenches.Core.Scripting.World;

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

    public static ClientStructure Convert(StructureDTO structure)
    {
        return new ClientStructure(
            Id: structure.Id, 
            Team: structure.Team, 
            Type: StructureTypes.Get(structure.Category), 
            Position: new (structure.X, structure.Y), 
            Health: structure.Health);
    }
}