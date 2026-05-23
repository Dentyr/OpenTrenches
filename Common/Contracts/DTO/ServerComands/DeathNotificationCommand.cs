using MessagePack;

namespace OpenTrenches.Common.Contracts.DTO.ServerComands;


[MessagePackObject]
public record class DeathNotificationCommand(
    [property: Key(0)] int Character
) : AbstractCommandDTO {}


[MessagePackObject]
public record class RespawnNotificationCommand(
    [property: Key(0)] int Character
) : AbstractCommandDTO {}