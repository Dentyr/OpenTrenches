using Godot;
using MessagePack;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.World;

namespace OpenTrenches.Common.Contracts.DTO.DataModel;

[MessagePackObject]
public record class WorldChunkDTO(
    [property: Key(0)] TileType[][] Gridmap,
    [property: Key(1)] TileConstructDTO[] Builds,
    [property: Key(2)] int X,
    [property: Key(3)] int Y
) : AbstractCreateDTO;
