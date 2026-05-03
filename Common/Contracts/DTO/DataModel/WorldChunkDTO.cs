using Godot;
using MessagePack;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.World;

namespace OpenTrenches.Common.Contracts.DTO.DataModel;

/// <summary>
/// Represents a chunk of tiles <paramref name="Gridmap"/> offset by <paramref name="XOffset"/>, <paramref name="YOffset"/>
/// </summary>
[MessagePackObject]
public record class WorldChunkDTO(
    [property: Key(0)] TileType[][] Gridmap,
    [property: Key(1)] int XOffset,
    [property: Key(2)] int YOffset
) : AbstractCreateDTO;
