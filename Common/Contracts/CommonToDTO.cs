using OpenTrenches.Common.Contracts.DTO;
using OpenTrenches.Common.World;

namespace OpenTrenches.Common.Contracts;

public static class CommonToDTO
{
    public static WorldChunkDTO Convert(ChunkRecord record)
        => new(record.Chunk.Select(tile => tile is null ? null : new TileRecord(TileType.Trench, 100, null)), record.X, record.Y);

    // public static WorldChunkDTO Convert(ChunkRecord record)
    //     => new(record.Chunk.Select(tile => tile is null ? null : Convert(tile)), record.X, record.Y);

    public static TileRecord Convert(Tile tile)
        => new(tile.Type, tile.Health, tile.Building is null ? null : Convert(tile.Building));
    
    public static BuildingRecord Convert(BuildStatus building)
        => new(building.BuildTarget, building.BuildProgress);
}


public static class CommonFromDTO
{
    public static ChunkRecord Convert(WorldChunkDTO record)
        // => new(new(), record.X, record.Y);
        => new(new Chunk((x, y) => record.Gridmap[x][y] is TileRecord notnull ? Convert(notnull) : null ), record.X, record.Y);

    public static Tile Convert(TileRecord tile)
        => new(tile.Tile, tile.Health, tile.Building is null ? null : Convert(tile.Building));

    public static BuildStatus Convert(BuildingRecord building)
        => new(building.BuildTarget, building.BuildProgress);
}