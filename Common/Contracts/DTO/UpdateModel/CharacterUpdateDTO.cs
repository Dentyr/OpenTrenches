using Godot;
using MessagePack;

namespace OpenTrenches.Common.Contracts.DTO.UpdateModel;

[MessagePackObject]
public record class CharacterUpdateDTO(CharacterAttribute Attribute, byte[] Payload, ushort TargetId) : AbstractUpdateDTO<CharacterAttribute>(Attribute, Payload)
{
    [Key(2)]
    public ushort TargetId { get; } = TargetId;

    public override bool Streamed => Attribute == CharacterAttribute.Position;
}

public enum CharacterAttribute : byte
{
    Position,
    Direction,
    Health,
    State,

    PrimarySlot,
}
