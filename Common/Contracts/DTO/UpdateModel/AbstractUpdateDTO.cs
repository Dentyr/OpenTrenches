using System;
using MessagePack;

namespace OpenTrenches.Common.Contracts.DTO.UpdateModel;

[MessagePackObject]
[Union(0, typeof(CharacterUpdateDTO))]
[Union(1, typeof(FirearmSlotUpdateDTO))]
[Union(2, typeof(PlayerUpdateDTO))]
[Union(10, typeof(WorldGridAttributeUpdateDTO))]
public abstract record class AbstractUpdateDTO
{
    [IgnoreMember]
    public virtual bool Streamed => false;
}


public abstract record class AbstractUpdateDTO<TAtt>(TAtt Attribute, byte[] Payload) : AbstractUpdateDTO where TAtt : struct, Enum
{
    [Key(0)]
    public TAtt Attribute { get; } = Attribute;
    [Key(1)]
    public byte[] Payload { get; } = Payload;
}