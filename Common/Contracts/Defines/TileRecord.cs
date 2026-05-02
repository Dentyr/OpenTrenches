using MessagePack;
using OpenTrenches.Common.World;

namespace OpenTrenches.Common.Contracts.Defines;


[MessagePackObject]
public record class TileRecord(
    [property: Key(0)] TileType? Tile,
    [property: Key(1)] int X,
    [property: Key(2)] int Y
);
