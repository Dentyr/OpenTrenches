using Godot;

namespace OpenTrenches.Common.Util;

public static class GeometryServices
{
    /// <summary>
    /// Makes a transform that just moves the origin to <paramref name="position"/>
    /// </summary>
    public static Transform2D MakeTranslate(Vector2 position)
        => new(Vector2.Right, Vector2.Down, position);
}