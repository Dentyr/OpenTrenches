using MessagePack;
using OpenTrenches.Common.Combat;

namespace OpenTrenches.Common.Contracts.DTO.PlayerCommands;

[MessagePackObject]
public record class PurchaseCommandRequest (
    [property: Key(0)] FirearmEnum Equipment
) : AbstractCommandDTO {}