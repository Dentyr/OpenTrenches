using OpenTrenches.Common.Contracts.DTO;
using OpenTrenches.Common.World;

namespace OpenTrenches.Common.Contracts;

public static class CommonToDTO
{
    public static WorldChunkDTO Convert(ChunkRecord record)
        => new(record.Chunk.Select(tile => tile is null ? null : Convert(tile)), record.X, record.Y);

    public static TileRecord Convert(Tile tile)
        => new(tile.Type, tile.Health, null);
}
public static class CommonFromDTO
{
    public static ChunkRecord Convert(WorldChunkDTO record)
        => new(new(), record.X, record.Y);
        //new(record.Chunk.Select(tile => tile is null ? null : Convert(tile)), record.X, record.Y);

    public static Tile Convert(TileRecord tile)
        => new(tile.Tile, tile.Health);
}