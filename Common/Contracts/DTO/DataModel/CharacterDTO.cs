using Godot;
using MessagePack;

namespace OpenTrenches.Common.Contracts.DTO.DataModel;

[MessagePackObject]
public record  class CharacterDTO(
    [property: Key(0)] ushort ID,
    [property: Key(1)] Vector3 Position,
    [property: Key(2)] float Health,
    [property: Key(3)] int Team
) : AbstractCreateDTO {}
