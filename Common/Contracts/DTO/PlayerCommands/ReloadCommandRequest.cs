using MessagePack;

namespace OpenTrenches.Common.Contracts.DTO.PlayerCommands;

[MessagePackObject]
public record class ReloadCommandRequest : AbstractCommandDTO
{}