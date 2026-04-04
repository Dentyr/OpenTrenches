using System;
using Godot;

namespace OpenTrenches.Core.Scene.World;
/// <summary>
/// <see cref="CharacterRenderer"/> component for a client's player character.
/// </summary>
public partial class FocusCamera : Camera2D
{
    public FocusCamera()
    {
        // Position = new Vector2(0, 0);
        // Zoom = new Vector2(0.4f, 0.4f);
    }
}