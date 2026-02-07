using MessagePack;

namespace OpenTrenches.Common.Contracts.DTO.ServerComands;
/// <summary>
/// Notifies client that all core state information has been transferred
/// </summary>
[MessagePackObject]
public record class InitializedNotificationCommand : AbstractCommandDTO
{}