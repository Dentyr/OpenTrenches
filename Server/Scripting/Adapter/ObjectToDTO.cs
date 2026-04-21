using OpenTrenches.Common.Contracts.DTO.DataModel;
using OpenTrenches.Server.Scripting.Player;
using OpenTrenches.Server.Scripting.Teams;
using OpenTrenches.Server.Scripting.World;

namespace OpenTrenches.Server.Scripting.Adapter;

public static class ObjectToDTO
{
    public static StructureDTO Convert(ServerStructure structure)
        => new (structure.Id, structure.Team.ID, structure.Position.X, structure.Position.Y, structure.Enum, structure.Hp);

    public static CharacterDTO Convert(Character character)
        => new (character.ID, character.Position, character.Hp, character.Team.ID, character.PrimarySlot.EquipmentEnum);

    public static TeamDTO Convert(Team team)
        => new (team.ID, team.Faction);
}