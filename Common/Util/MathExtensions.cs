using System;
using Godot;

public static class MathExtensions
{
    public static Vector3 HSpread(this Vector3 dir, float minuteOfAngle)
    {
        dir = dir.Normalized();

        float maxAngleRad = (float)(minuteOfAngle / 60 * Math.PI / 180);
        float yaw = (GD.Randf() - 0.5f) * 2 * maxAngleRad;

        // Horizontal-only spread: rotate around the world up axis (XZ plane).
        return dir.Rotated(Vector3.Up, yaw).Normalized();
    }

    public static Vector2 Spread(this Vector2 dir, float minuteOfAngle)
    {
        dir = dir.Normalized();

        float maxAngleRad = (float)(minuteOfAngle / 60 * Math.PI / 180);
        float angle = (GD.Randf() - 0.5f) * 2 * maxAngleRad;

        return dir.Rotated(angle);
    }

    /// <summary>
    /// Returns the greater between the Y distance and X distance between <paramref name="position"/> and <paramref name="target"/>
    /// </summary>
    public static float ChebyshevDistanceTo(this Vector2 position, Vector2 target)
    {
        return Math.Max(Math.Abs(position.X - target.X), Math.Abs(position.Y - target.Y));
    }


    public static Rect2I Translate(this Rect2I rect, Vector2I position)
    {
        return new(rect.Position + position, rect.Size);
    }
}
