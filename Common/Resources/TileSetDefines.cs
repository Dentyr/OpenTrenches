using Godot;

namespace OpenTrenches.Common.Resources;

public static class TileSetDefines
{
    public static int DefualtTerrainAtlasID = 1;
    /// <summary>
    /// ID for atlases that contain misc cells for tile sets, such as empty tiles or default tiles.
    /// </summary>
    public static int MiscCellAltasID = 101;


    /// <summary>
    /// The atlas position of the tile that is continugous (set, and surrounded on all sides by similarly set tiles)
    /// </summary>
    public static Vector2I FillAtlasPosition = new(1, 1);

    /// <summary>
    /// The atlas position of the tile that is meant for unset tiles
    /// </summary>
    public static Vector2I MiscClearedPosition = new(0, 0);
}
