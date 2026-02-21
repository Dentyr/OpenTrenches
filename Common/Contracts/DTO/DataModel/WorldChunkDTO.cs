using Godot;
using MessagePack;
using OpenTrenches.Common.Contracts.Defines;

namespace OpenTrenches.Common.Contracts.DTO.DataModel;

[MessagePackObject]
public record class WorldChunkDTO(TileRecord?[][] Gridmap, byte X, byte Y) : AbstractCreateDTO
{
    [Key(0)]
    public TileRecord?[][] Gridmap { get; } = Gridmap;
    [Key(1)]
    public byte X { get; } = X;
    [Key(2)]
    public byte Y { get; } = Y;
}
