using MessagePack;

namespace OpenTrenches.Common.Contracts.DTO.PlayerCommands;

[MessagePackObject]
public record class RespawnCommandRequest(
    /// <summary>
    /// Which camp to stay at
    /// </summary>
    [property: Key(0)] int CampId
)
: AbstractCommandDTO
{}