using MessagePack;

namespace OpenTrenches.Common.Contracts.DTO.UpdateModel;

[MessagePackObject]
public record class StructureUpdateDTO
(
    StructureAttribute Attribute, 
    byte[] Payload,
    [property: Key(2)] int X,
    [property: Key(3)] int Y
) : AbstractUpdateDTO<StructureAttribute>(Attribute, Payload)
{}

public enum StructureAttribute : byte
{
    Health,
}
