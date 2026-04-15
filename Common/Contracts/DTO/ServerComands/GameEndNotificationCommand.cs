using MessagePack;

namespace OpenTrenches.Common.Contracts.DTO.ServerComands;

[MessagePackObject]
public record GameEndNotificationCommand(
    [property: Key(0)] int VictorTeam
) : AbstractCommandDTO {}