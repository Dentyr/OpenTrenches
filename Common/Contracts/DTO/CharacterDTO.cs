using Godot;
using MessagePack;

namespace OpenTrenches.Common.Contracts.DTO;

[MessagePackObject]
public record  class CharacterDTO(ushort ID, Vector3 Position, float Health) : AbstractCreateDTO
{
    [Key(0)]
    public ushort ID { get; } = ID;

    [Key(1)]
    public Vector3 Position { get; } = Position;

    [Key(2)]
    public float Health { get; } = Health;
}
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