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
}
