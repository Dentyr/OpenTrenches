using System;
using System.Collections.Generic;
using Godot;
using OpenTrenches.Scripting.Player;

namespace OpenTrenches.Scene.World;

public partial class WorldNode : Node3D
{
    //* Camera
    private ushort _focusCharacter;

    private FocusCamera Camera { get; }

    //* Things
    private Dictionary<ushort, CharacterNode> _characters = [];
    private Node3D _characterLayer { get; }

    private bool ChildPhysicsEnabled { get; set; } = true;

    public WorldNode()
    {
        _characterLayer = new()
        {
            Name = "Characters",
        };
        AddChild(_characterLayer);
        
        Camera = new();
        AddChild(Camera);
    }

    public void AddCharacter(ushort id, Character character)
    {
        Console.WriteLine("NEW CHAR");
        if (_characters.TryAdd(id, new(id, character)))
        {
            CharacterNode node = _characters[id];
            _characterLayer.AddChild(node);
            node.SetPhysicsProcess(ChildPhysicsEnabled);
        }
    }
    public void DisablePhysics()
    {
        ChildPhysicsEnabled = false;
        foreach(var node in _characterLayer.GetChildren()) node.SetPhysicsProcess(false);
    }

    public void CameraFocusCharacter(ushort id)
    {
        Console.WriteLine(id);
        _focusCharacter = id;

    }

    public override void _Process(double delta)
    {
        if (_characters.TryGetValue(_focusCharacter, out var thing)) 
        {
            Camera.Focus(thing);
        }
    }
}