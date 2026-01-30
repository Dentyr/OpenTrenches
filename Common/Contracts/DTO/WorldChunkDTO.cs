using Godot;
using MessagePack;

namespace OpenTrenches.Common.Contracts.DTO;

[MessagePackObject]
public record class WorldChunkDTO(TileRecord?[][] Gridmap, byte X, byte Y) : AbstractDTO
{
    [Key(0)]
    public TileRecord?[][] Gridmap { get; } = Gridmap;
    [Key(1)]
    public byte X { get; } = X;
    [Key(2)]
    public byte Y { get; } = Y;
}
public enum TileType : byte
{
    Trench,
    
}
public enum GridTilettribute : byte
{
    Terrain,
    Health,
    BuildProgress
}



[MessagePackObject]
public record class WorldGridAttributeUpdateDTO(GridTilettribute Attribute, byte ChunkX, byte ChunkY, byte X, byte Y, byte[] Payload) : AbstractUpdateDTO
{
    public static WorldGridAttributeUpdateDTO CreateTerrain(byte ChunkX, byte ChunkY, byte X, byte Y, TileType Target) => new(GridTilettribute.Terrain, ChunkX, ChunkY, X, Y, Serialization.Serialize(Target));
    public static WorldGridAttributeUpdateDTO CreateHealth(byte ChunkX, byte ChunkY, byte X, byte Y, float Target) => new(GridTilettribute.Health, ChunkX, ChunkY, X, Y, Serialization.Serialize(Target));
    public static WorldGridAttributeUpdateDTO CreateBuildProgress(byte ChunkX, byte ChunkY, byte X, byte Y, float Target) => new(GridTilettribute.BuildProgress, ChunkX, ChunkY, X, Y, Serialization.Serialize(Target));
    
    [Key(0)]
    public GridTilettribute Attribute { get; } = Attribute;
    [Key(1)]
    public byte ChunkX { get; } = ChunkX;
    [Key(2)]
    public byte ChunkY { get; } = ChunkY;
    [Key(3)]
    public byte X { get; } = X;
    [Key(4)]
    public byte Y { get; } = Y;
    [Key(5)]
    public byte[] Payload { get; } = Payload;

}
