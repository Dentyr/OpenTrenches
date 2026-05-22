using Godot;

namespace OpenTrenches.Core.Scene.World;

/// <summary>
/// Replaces the ordinary mouse cursor for an indicator of recoil
/// </summary>
public partial class Cursor : Node2D
{
    public float Recoil { get; set; }

    public override void _Draw()
    {
        float gap = Mathf.Lerp(5f, 28f, Mathf.Clamp(Recoil / 200f, 0f, 5f));
        float tick = 8f;

        Color color = new(0.9f, 0.9f, 0.8f, 0.8f);

        DrawLine(new Vector2(-gap - tick, 0), new Vector2(-gap, 0), color, 2f);
        DrawLine(new Vector2(gap, 0), new Vector2(gap + tick, 0), color, 2f);
        DrawLine(new Vector2(0, -gap - tick), new Vector2(0, -gap), color, 2f);
        DrawLine(new Vector2(0, gap), new Vector2(0, gap + tick), color, 2f);

        DrawCircle(Vector2.Zero, 2f, color);
    }

    public void SetRecoil(float recoil)
    {
        Recoil = recoil;
        QueueRedraw();
    }
}