using System;
using Godot;
using OpenTrenches.Core.Scene.Resources;
using OpenTrenches.Core.Scripting.Libraries;
using OpenTrenches.Core.Scripting.Player;

namespace OpenTrenches.Core.Scene.World;

public partial class CharacterNode : Node3D
{
    private Character Character { get; }
    public CharacterNode(Character Character)
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
