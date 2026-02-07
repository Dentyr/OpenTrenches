using MessagePack;
using OpenTrenches.Common.Factions;

namespace OpenTrenches.Common.Contracts.DTO.DataModel;

/// <param name="Faction">The identifier of the faction type, defines the description and ability of the faction</param>
/// <param name="ID">The identifier of the team in the game, defines a side in a game instance</param>
[MessagePackObject]
public record class TeamDTO(
    [property: Key(0)] int ID,
    [property: Key(1)] FactionEnum Faction
) : AbstractCreateDTO {}