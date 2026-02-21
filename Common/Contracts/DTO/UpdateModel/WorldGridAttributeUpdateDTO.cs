using Godot;
using MessagePack;

namespace OpenTrenches.Common.Contracts.DTO.UpdateModel;

[MessagePackObject]
public record class WorldGridAttributeUpdateDTO(GridTilettribute Attribute, int X, int Y, byte[] Payload) : AbstractUpdateDTO
{
    public static WorldGridAttributeUpdateDTO CreateTerrain(int X, int Y, TileType Target) => new(GridTilettribute.Terrain, X, Y, Serialization.Serialize(Target));
    public static WorldGridAttributeUpdateDTO CreateHealth(int X, int Y, float Target) => new(GridTilettribute.Health, X, Y, Serialization.Serialize(Target));
    public static WorldGridAttributeUpdateDTO CreateBuildProgress(int X, int Y, float Target) => new(GridTilettribute.BuildProgress, X, Y, Serialization.Serialize(Target));
    
    [Key(0)]
    public GridTilettribute Attribute { get; } = Attribute;
    [Key(1)]
    public int X { get; } = X;
    [Key(2)]
    public int Y { get; } = Y;
    [Key(3)]
    public byte[] Payload { get; } = Payload;

}
public enum TileType : byte
{
    Clear = 0,
    Trench,
    
}
public enum GridTilettribute : byte
{
    Terrain,
    Health,
    BuildProgress
}
