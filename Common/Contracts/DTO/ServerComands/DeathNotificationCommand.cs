using MessagePack;

namespace OpenTrenches.Common.Contracts.DTO.ServerComands;


[MessagePackObject]
public record class DeathNotificationCommand(
    [property: Key(0)] uint Character
) : AbstractCommandDTO {}


[MessagePackObject]
public record class RespawnNotificationCommand(
    [property: Key(0)] uint Character
) : AbstractCommandDTO {}