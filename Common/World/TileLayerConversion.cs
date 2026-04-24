namespace OpenTrenches.Common.World;

/// <summary>
/// Converts <see cref="TileType"/>s to its corresponding <see cref="WorldLayer"/>
/// </summary>
public static class TileLayerConversion
{
    public static WorldLayer LayerOf(TileType? tile)
    {
        switch (tile)
        {
            case TileType.Clear:
            case null:
            default:
                return WorldLayer.Ground;
            case TileType.Trench:
                return WorldLayer.Trench;
        }
    }
}