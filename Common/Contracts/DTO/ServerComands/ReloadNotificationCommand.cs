using MessagePack;

namespace OpenTrenches.Common.Contracts.DTO.ServerComands;

[MessagePackObject]
public record class ReloadNotificationCommand(
    [property: Key(0)] ushort Character
) : AbstractCommandDTO
{}