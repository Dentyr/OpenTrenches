using MessagePack;

namespace OpenTrenches.Common.Contracts.DTO.UpdateModel;

[MessagePackObject]
public record class StructureUpdateDTO
(
    StructureAttribute Attribute, 
    byte[] Payload,
    [property: Key(2)] int Id
) : AbstractUpdateDTO<StructureAttribute>(Attribute, Payload);

public enum StructureAttribute : byte
{
    Health,
}
