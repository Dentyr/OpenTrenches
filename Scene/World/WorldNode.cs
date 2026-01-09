using System;
using Godot;
using OpenTrenches.Scripting.Player;

namespace OpenTrenches.Scene.World;

public partial class WorldNode : Node3D
{
    private Node3D _characterLayer { get; }
    private bool ChildPhysicsEnabled { get; set; } = true;

    public WorldNode()
    {
        _characterLayer = new()
        {
            Name = "Characters",
        };
        AddChild(_characterLayer);
        
    }

    public void AddCharacter(ushort id, Character character)
    {
        CharacterNode3D node = new(character);
        _characterLayer.AddChild(node);
        node.SetPhysicsProcess(ChildPhysicsEnabled);
    }
    public void DisablePhysics()
    {
        ChildPhysicsEnabled = false;
    }
}