using Godot;
using OpenTrenches.Common.Contracts.Defines;

namespace OpenTrenches.Common.World;

/// <summary>
/// Translates between exact positions/cells and areas.
/// Areas represent upper level square groupings of cells for AI behavior
/// </summary>
public static class AreaTranslationService
{
    public static Vector2 GetAreaCenter(Vector2I area) => new Vector2(area.X + 0.5f, area.Y + 0.5f) * CommonDefines.AreaSize;
}