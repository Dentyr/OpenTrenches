using System;
using Godot;
using OpenTrenches.Scene.Resources;
using OpenTrenches.Scripting.Libraries;
using OpenTrenches.Scripting.Player;

namespace OpenTrenches.Scene;

public partial class CharacterNode3D : CharacterBody3D
{
    private Character Character { get; }
    public CharacterNode3D(Character Character)
    {
        this.Character = Character;
        Position = Character.Position;
        AddChild(new MeshInstance3D()
        {
            Mesh = Meshes.Dirt,
            MaterialOverride = Materials.PinkDebug,
        });
        AddChild(new CollisionShape3D()
        {
            Shape = new BoxShape3D()
            {
                Size = new(1, 2, 1),
            }
        });

    }

    public override void _Process(double delta)
    {        
        this.Position = Character.Position;
    }

    public override void _PhysicsProcess(double delta)
    {
        this.Position = Character.Position;
        this.Velocity += SceneDefines.Physics.g * (float)delta;
        Velocity = new(Character.Movement.X * (float)delta, Velocity.Y, Character.Movement.Z * (float)delta);
        this.MoveAndSlide();
        Character.Position = this.Position;
    }
}
