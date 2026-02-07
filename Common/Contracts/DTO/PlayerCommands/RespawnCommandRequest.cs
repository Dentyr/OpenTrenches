using MessagePack;

namespace OpenTrenches.Common.Contracts.DTO.PlayerCommands;

[MessagePackObject]
public record class RespawnCommandRequest : AbstractCommandDTO
{}