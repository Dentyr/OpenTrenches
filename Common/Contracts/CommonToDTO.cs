using System.Linq;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Contracts.DTO.DataModel;
using OpenTrenches.Common.World;

namespace OpenTrenches.Common.Contracts;

public static class CommonToDTO
{
    public static WorldChunkDTO Convert(ChunkRecord<Chunk> record)
        => new(
            record.Chunk.CopyTiles(), 
            record.Chunk.GetActiveEarthworks()
                .Select(record => new TileConstructDTO(record.X, record.Y, record.Status.Progress, record.Status.Target))
                .ToArray(), 
            record.X, 
            record.Y
        );

    // public static WorldChunkDTO Convert(ChunkRecord record)
    //     => new(record.Chunk.Select(tile => tile is null ? null : Convert(tile)), record.X, record.Y);
    
    public static TileConstructDTO Convert(int x, int y, TileConstruction building)
        => new(x, y, building.Progress, building.Target);
}


public static class CommonFromDTO
{
    public static ChunkRecord<Chunk> Convert(WorldChunkDTO record)
        => new(
            new Chunk(record.Gridmap, 
                record.Builds.Select(
                    // TODO Decay is only needed serverside, so it is given 0 here. Maybe split chunk into serverchunk and clientchunk if more differences emerge
                    dto => new TileConstructionRecord(dto.X, dto.Y, new(dto.Tile, dto.Progress, 0))
                )
            ), 
            record.X, 
            record.Y);


    public static TileConstruction Convert(TileConstructDTO dto)
        => new(dto.Tile, dto.Progress);
}