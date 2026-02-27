using MessagePack;

namespace OpenTrenches.Common.Contracts.DTO.UpdateModel;

public enum FirearmSlotAttribute : byte
{
    AmmoLoaded,
    AmmoStored,
    Recoil,
}


[MessagePackObject]
public record class FirearmSlotUpdateDTO(FirearmSlotAttribute Attribute, byte[] Payload) : AbstractUpdateDTO<FirearmSlotAttribute>(Attribute, Payload) {}