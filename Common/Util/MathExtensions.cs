using System;
using Godot;

public static class MathExtensions
{

    private static Vector3 AnyPerpendicular(this Vector3 dir)
    {
        dir = dir.Normalized();
        // Pick an axis least aligned with dir to avoid near-zero cross
        Vector3 a = Mathf.Abs(dir.Y) < 0.98f ? Vector3.Up : Vector3.Right;
        return dir.Cross(a).Normalized();
    }
    public static Vector3 BoxSpread(this Vector3 dir, float minuteOfAngle)
    {
        dir = dir.Normalized();
        Vector3 right = AnyPerpendicular(dir);
        Vector3 up = dir.Cross(right).Normalized();

        float maxAngleRad = (float)(minuteOfAngle / 60 * Math.PI / 180);

        float x = (GD.Randf()-0.5f) * 2 * maxAngleRad;
        float y = (GD.Randf()-0.5f) * 2 * maxAngleRad;

        return (dir + right * x + up * y).Normalized();
    }
}