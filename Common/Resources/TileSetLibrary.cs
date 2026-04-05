using Godot;
using System;

public partial class TileSetLibrary : Node
{
    public static readonly TileSet GrassTileSet = ResourceLoader.Load<TileSet>("Common/Resources/TileSet/GrassTileSet.tres");
    public static readonly TileSet WallTileSet  = ResourceLoader.Load<TileSet>("Common/Resources/TileSet/WallTileSet.tres");
}
