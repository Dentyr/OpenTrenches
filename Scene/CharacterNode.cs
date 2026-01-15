using System;
using Godot;
using OpenTrenches.Scene.Resources;
using OpenTrenches.Scripting.Libraries;
using OpenTrenches.Scripting.Player;

namespace OpenTrenches.Scene.World;

public partial class CharacterNode : Node3D
{
    private ushort Id { get; }
    private Character Character { get; }
    public CharacterNode(ushort Id, Character Character)
    {
        this.Id = Id;
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


        AddChild(new Label()
        {
            Text = "Label Test",
        });
    }

    public override void _Process(double delta)
    {        
        Position = Character.Position;
    }

}
