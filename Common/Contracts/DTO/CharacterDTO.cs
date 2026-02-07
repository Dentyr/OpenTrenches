using Godot;
using MessagePack;

namespace OpenTrenches.Common.Contracts.DTO;

[MessagePackObject]
public record  class CharacterDTO(
    [property: Key(0)] ushort ID,
    [property: Key(1)] Vector3 Position,
    [property: Key(2)] float Health,
    [property: Key(3)] int Team
) : AbstractCreateDTO {}
public enum CharacterAttribute : byte
{
    Position,
    Direction,
    Health,
    Cooldown,
    State
}


[MessagePackObject]
public record class CharacterUpdateDTO(CharacterAttribute Attribute, byte[] Payload, ushort TargetId) : AbstractUpdateDTO<CharacterAttribute>(Attribute, Payload)
{
    [Key(2)]
    public ushort TargetId { get; } = TargetId;

    public override bool Streamed => Attribute == CharacterAttribute.Position;
}