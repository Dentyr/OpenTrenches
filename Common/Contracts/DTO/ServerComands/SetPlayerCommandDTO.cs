
using Godot;
using MessagePack;
namespace OpenTrenches.Common.Contracts.DTO;

[MessagePackObject]
public record class SetPlayerCommandDTO(
    [property: Key(0)] ushort PlayerID,
    [property: Key(1)] int AmmoLoaded,
    [property: Key(2)] int AmmoStored,
    [property: Key(3)] int Logistics
) : AbstractCommandDTO {}
