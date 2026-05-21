using System;
using System.Collections.Generic;
using Godot;

namespace OpenTrenches.Common.World;

/// <summary>
/// Executes geometric queries on ITileArray2Ds
/// </summary>
public static class TileArrayGeometryService
{
    /// <summary>
    /// returns true if the line from <paramref name="origin"/> to <paramref name="destination"/> intersects any tiles of type <paramref name="tile"/>
    /// </summary>
    public static bool LineContainsTile(ITileArray2D tiles, Vector2 origin, Vector2 destination, TileType tile)
    {
        foreach (Vector2I cell in GetCellsInLine(origin, destination))
        {
            // if legal cell and is tile, return true
            if (cell.X < tiles.SizeX && 
                cell.Y < tiles.SizeY && 
                cell.X >= 0 && cell.Y >= 0
                && tiles[cell.X, cell.Y] == tile)
            {
                return true;
            }
        }
        return false;
    }

    private static IEnumerable<Vector2I> GetCellsInLine(Vector2 origin, Vector2 destination)
    {
        int x = (int)MathF.Floor(origin.X);
        int y = (int)MathF.Floor(origin.Y);
        int endX = (int)MathF.Floor(destination.X);
        int endY = (int)MathF.Floor(destination.Y);

        yield return new Vector2I(x, y);

        float dx = destination.X - origin.X;
        float dy = destination.Y - origin.Y;

        if (dx == 0f && dy == 0f)
        {
            yield break;
        }

        int stepX = MathF.Sign(dx);
        int stepY = MathF.Sign(dy);
        float tDeltaX = stepX == 0 ? float.PositiveInfinity : 1f / MathF.Abs(dx);
        float tDeltaY = stepY == 0 ? float.PositiveInfinity : 1f / MathF.Abs(dy);
        float tMaxX = stepX switch
        {
            > 0 => (x + 1 - origin.X) / dx,
            < 0 => (origin.X - x) / -dx,
            _ => float.PositiveInfinity
        };
        float tMaxY = stepY switch
        {
            > 0 => (y + 1 - origin.Y) / dy,
            < 0 => (origin.Y - y) / -dy,
            _ => float.PositiveInfinity
        };

        while (x != endX || y != endY)
        {
            if (tMaxX < tMaxY)
            {
                x += stepX;
                tMaxX += tDeltaX;
                yield return new Vector2I(x, y);
            }
            else if (tMaxY < tMaxX)
            {
                y += stepY;
                tMaxY += tDeltaY;
                yield return new Vector2I(x, y);
            }
            else
            {
                int nextX = x + stepX;
                int nextY = y + stepY;
                yield return new Vector2I(nextX, y);
                yield return new Vector2I(x, nextY);
                x = nextX;
                y = nextY;
                tMaxX += tDeltaX;
                tMaxY += tDeltaY;
                yield return new Vector2I(x, y);
            }
        }
    }
}
