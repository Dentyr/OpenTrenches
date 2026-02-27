using Godot;
using MessagePack;
using OpenTrenches.Common.Combat;

namespace OpenTrenches.Common.Contracts.DTO.DataModel;

[MessagePackObject]
public record  class CharacterDTO(
    [property: Key(0)] ushort ID,
    [property: Key(1)] Vector3 Position,
    [property: Key(2)] float Health,
    [property: Key(3)] int Team,
    [property: Key(4)] FirearmEnum? Primary
) : AbstractCreateDTO {}
