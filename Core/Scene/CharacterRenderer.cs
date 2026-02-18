using System;
using Godot;
using OpenTrenches.Common.Resources;
using OpenTrenches.Core.Scripting.Player;

namespace OpenTrenches.Core.Scene.World;

public partial class CharacterRenderer : Node3D
{
    private Character Character { get; }
    public CharacterRenderer(Character Character)
    {
        this.Character = Character;
        Position = Character.Position;
        AddChild(new MeshInstance3D()
        {
            Mesh = Meshes.Dirt,
            MaterialOverride = Character.OnPlayerTeam ? Materials.WhiteDebug : Materials.RedDebug,
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
        Position = Character.Position;
        Character.Process((float)delta);
    }

}
