using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using OpenTrenches.Core.Scripting.Player;

namespace OpenTrenches.Scene.World;

public partial class WorldNode : Node3D
{
    //* Characters
    private Dictionary<ushort, (CharacterNode CharacterNode, Label Label)> _characters = [];
    private Node3D _characterLayer { get; }


    //* UI

    private Node _characterUILayer;
    

    private bool ChildPhysicsEnabled { get; set; } = true;

    public WorldNode()
    {
        _characterLayer = new()
        {
            Name = "Characters",
        };
        AddChild(_characterLayer);
        

        _characterUILayer = new();
        AddChild(_characterUILayer);
    }

    public void AddCharacter(ushort id, Character character)
    {
        if (_characters.TryAdd(id, new(new CharacterNode(id, character), new Label())))
        {
            CharacterNode node = _characters[id].CharacterNode;
            Label label = _characters[id].Label;
            _characterLayer.AddChild(node);
            _characterUILayer.AddChild(label);
            node.SetPhysicsProcess(ChildPhysicsEnabled);
        }
    }
    public void DisablePhysics()
    {
        ChildPhysicsEnabled = false;
        foreach(var node in _characterLayer.GetChildren()) node.SetPhysicsProcess(false);
    }

    public void AddPlayerComponents(ushort id)
    {
        if (_characters.TryGetValue(id, out var tuple)) 
        {
            tuple.CharacterNode.AddChild(new FocusCamera());
        }

    }

    public override void _Process(double delta)
    {

        //* ui elements

        foreach(var tuple in _characters.Values) 
        {
            tuple.Label.Position = new (tuple.CharacterNode.Position.X, tuple.CharacterNode.Position.Z);
            // tuple.Label.Position -= ne
        }
    }
}