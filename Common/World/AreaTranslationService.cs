using Godot;
using OpenTrenches.Common.Contracts.Defines;

namespace OpenTrenches.Common.World;

/// <summary>
/// Translates between exact positions/cells and areas.
/// Areas represent upper level square groupings of cells for AI behavior
/// </summary>
public static class AreaTranslationService
{
    public static Vector2I GetAreaFromForward(int direction, int lane, int forward)
    {
        if (direction > 0)
            return new(forward, lane);
        else
            return new(CommonDefines.WorldLengthArea - 1 - forward, lane);
    }

    public static Vector2 GetAreaCenter(Vector2I area) => new Vector2(area.X + 0.5f, area.Y + 0.5f) * CommonDefines.AreaSize;
}
