using System;
using Godot;

namespace OpenTrenches.Scene.World;
/// <summary>
/// <see cref="CharacterNode"/> component for a client's player character.
/// </summary>
public partial class FocusCamera : Camera3D
{
    public FocusCamera()
    {
        Position = new Vector3(0, 20, 0);
        Rotation = new Vector3(1.5f * (float)Math.PI, 0, 0);
    }

    internal void Focus(Node3D Target)
    {
        Position = Target.Position + new Vector3(0, 20, 0);
        Rotation = new Vector3(1.5f * (float)Math.PI, 0, 0);
    }
}