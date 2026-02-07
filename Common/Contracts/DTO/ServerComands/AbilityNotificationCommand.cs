

using MessagePack;

namespace OpenTrenches.Common.Contracts.DTO.ServerComands;

/// <summary>
/// Notifies the activation of an ability
/// </summary>
/// <param name="Idx">Index of ability</param>
[MessagePackObject]
public record AbilityNotificationCommand(
    [property: Key(0)] uint Character,
    [property: Key(1)] int Idx
) : AbstractCommandDTO {}