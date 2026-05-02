using MessagePack;

namespace OpenTrenches.Common.Contracts.DTO.UpdateModel;

[MessagePackObject]
public record class TileConstructUpdateDTO(
    [property: Key(0)] int X,
    [property: Key(1)] int Y,
    [property: Key(2)] float Progress
);