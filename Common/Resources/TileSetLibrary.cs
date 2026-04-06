using Godot;
using System;

public static class TileSetLibrary
{
    public static readonly TileSet TrenchTileSet = ResourceLoader.Load<TileSet>("Common/Resources/TileSet/TrenchTileSet.tres");
    public static readonly TileSet GrassTileSet = ResourceLoader.Load<TileSet>("Common/Resources/TileSet/GrassTileSet.tres");
    public static readonly TileSet WallTileSet  = ResourceLoader.Load<TileSet>("Common/Resources/TileSet/WallTileSet.tres");
}
