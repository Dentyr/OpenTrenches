using System.Linq;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Contracts.DTO.DataModel;
using OpenTrenches.Common.World;
using OpenTrenches.Core.Scene.World;
using OpenTrenches.Server.Scene.World;

namespace OpenTrenches.Common.Contracts;

public static class CommonToDTO
{

    // public static WorldChunkDTO Convert(ChunkRecord record)
    //     => new(record.Chunk.Select(tile => tile is null ? null : Convert(tile)), record.X, record.Y);
    
    public static TileConstructDTO Convert(int x, int y, TileConstruction building)
        => new(x, y, building.Progress, building.Target);
}


public static class CommonFromDTO
{

    public static TileConstruction Convert(TileConstructDTO dto)
        => new(dto.Tile, dto.Progress);
}