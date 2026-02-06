

using MessagePack;

namespace OpenTrenches.Common.Contracts.DTO.PlayerCommands;

/// <summary>
/// Notifies the requested activation of an ability
/// </summary>
/// <param name="Idx">Index of ability</param>
[MessagePackObject]
public record class UseAbilityCommandRequest(
    [property: Key(0)] int Idx
) : AbstractCommandDTO {}