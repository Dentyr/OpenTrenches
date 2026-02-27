
using Godot;
using MessagePack;
namespace OpenTrenches.Common.Contracts.DTO;

[MessagePackObject]
public record class SetPlayerCommandDTO(
    [property: Key(0)] ushort PlayerID
) : AbstractCommandDTO {}
