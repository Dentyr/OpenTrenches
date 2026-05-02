using MessagePack;
using OpenTrenches.Common.World;

namespace OpenTrenches.Common.Contracts.DTO.DataModel;

/// <summary>
/// Represents a tile being modified to some other tile, such as a trench being dug
/// </summary>
[MessagePackObject]
public record class TileConstructDTO(
    [property: Key(0)] int X,
    [property: Key(1)] int Y,
    [property: Key(2)] float Progress,
    [property: Key(3)] TileType Tile
);