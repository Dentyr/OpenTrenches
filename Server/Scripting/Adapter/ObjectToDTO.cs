using OpenTrenches.Common.Contracts.DTO.DataModel;
using OpenTrenches.Server.Scripting.Player;
using OpenTrenches.Server.Scripting.Teams;

namespace OpenTrenches.Server.Scripting.Adapter;

public static class ObjectToDTO
{
    public static CharacterDTO Convert(Character character)
        => new (character.ID, character.MovementVelocity, character.Health, character.Team.ID);

    public static TeamDTO Convert(Team team)
        => new (team.ID, team.Faction);
}