#if DEBUG
using System;
using Godot;

namespace OpenTrenches.Server.Scene;

public partial class DebugCamera : Camera2D
{

    private int _zoomLevel = 0;
    private float GetZoomFactor() => (float)Math.Pow(2, _zoomLevel);

    private void UpdateZoom(int zoom)
    {
        _zoomLevel = zoom;
        float factor = GetZoomFactor();
        Zoom = new(factor, factor);
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
        {
            switch(keyEvent.Keycode)
            {
                case Key.Q:
                    UpdateZoom(_zoomLevel + 1);
                    break;
                case Key.E:
                    UpdateZoom(_zoomLevel - 1);
                    break;
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 movement = Vector2.Zero;
        if (Input.IsKeyPressed(Key.W)) movement += Vector2.Up;
        if (Input.IsKeyPressed(Key.A)) movement += Vector2.Left;
        if (Input.IsKeyPressed(Key.S)) movement += Vector2.Down;
        if (Input.IsKeyPressed(Key.D)) movement += Vector2.Right;

        Position += movement / GetZoomFactor() * 10;
    }
}
#endif