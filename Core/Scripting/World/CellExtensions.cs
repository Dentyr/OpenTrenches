using Godot;

namespace OpenTrenches.Core.Scripting.World;

public static class CellExtensions
{
    /// <summary>
    /// Converts a cell position to a world position by finding the center of the cell.
    /// </summary>
    public static Vector2 CellToPosition(this Vector2I cell)
    {
        return new Vector2(0.5f, 0.5f) + cell;
    }
}