using MessagePack;

namespace OpenTrenches.Common.Contracts.DTO.UpdateModel;

public enum FirearmSlotAttribute : byte
{
    AmmoLoaded,
    AmmoStored,
    FirearmChanged,
}


[MessagePackObject]
public record class FirearmSlotUpdateDTO(FirearmSlotAttribute Attribute, byte[] Payload, ushort TargetId) : AbstractUpdateDTO<FirearmSlotAttribute>(Attribute, Payload)
{
    [Key(2)]
    public ushort TargetId { get; } = TargetId;

    public override bool Streamed => false;
}