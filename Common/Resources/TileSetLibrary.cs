using Godot;
using System;

public partial class TileSetLibrary : Node
{
    public static TileSet GrassTileSet { get; } = ResourceLoader.Load<TileSet>("Common/Resources/TileSet/GrassTileSet.tres");
}
