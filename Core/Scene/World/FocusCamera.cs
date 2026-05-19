using System;
using Godot;

namespace OpenTrenches.Core.Scene.World;
/// <summary>
/// <see cref="CharacterRenderer"/> component for a client's player character.
/// </summary>
public partial class FocusCamera : Node2D
{
    /// <summary>
    /// How far the camera should see
    /// </summary>
    private const float ViewMultiplier = 1.5f;

    private Line2D _aimLine;
    private Camera2D _camera;

    private Vector2 _viewVector
    {
        get => _camera.Position / ViewMultiplier;
        set
        {
            
            float zoom = 1f / (1f + (value.Length() / 1000f));
            _camera.Position = value * ViewMultiplier;
            _camera.Zoom = Vector2.One * zoom;

            Vector2 mouseWorldPosition = _camera.Position + (value / zoom);
            _aimLine.Points = [Vector2.Zero, mouseWorldPosition];
        }
    }

    private float _moveVelocity = 0;

    public FocusCamera()
    {
        _aimLine = new()
        {
            Width = 2f,
            DefaultColor = new(0.6f, 0.6f, 0.6f, 1f),
        };
        AddChild(_aimLine);

        _camera = new();
        AddChild(_camera);
        // Position = new Vector2(0, 0);
        // Zoom = new Vector2(0.4f, 0.4f);
    }

    public override void _Process(double delta)
    {
        Vector2 center = GetViewportRect().Size / 2f;

        if (Input.IsMouseButtonPressed(MouseButton.Right))
        {
            _viewVector = GetViewport().GetMousePosition() - center;
            if (!_aimLine.Visible) _aimLine.Visible = true;
            // _aimLine.Points = [Vector2.Zero, Position];
        }
        else
        {
            if (_aimLine.Visible) _aimLine.Visible = false;
        }
    }
}
