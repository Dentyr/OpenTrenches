using System.Collections.Generic;
using Godot;
using OpenTrenches.Core.Scripting.Player;
using OpenTrenches.Core.Scene.Combat;
using OpenTrenches.Common.World;

namespace OpenTrenches.Core.Scene.World;

public partial class WorldNode : Node3D
{
    //* Characters
    private readonly Dictionary<ushort, (CharacterNode CharacterNode, CharacterFloat Label)> _characters = [];
    private Node3D _characterLayer { get; }

    //* tiles
    private ChunkLayer ChunkLayer { get; }

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

        ChunkLayer = new()
        {
            
        };
        AddChild(ChunkLayer);
        

        _characterUILayer = new();
        AddChild(_characterUILayer);
    }

    public void AddChunk(ChunkRecord record)
    {
        ChunkLayer.SetChunk(record);
    }

    public void AddCharacter(Character character)
    {
        if (_characters.TryAdd(character.ID, new(new CharacterNode(character), new CharacterFloat(character))))
        {
            CharacterNode node = _characters[character.ID].CharacterNode;
            _characterLayer.AddChild(node);

            CharacterFloat label = _characters[character.ID].Label;
            _characterUILayer.AddChild(label);
            
            node.SetPhysicsProcess(ChildPhysicsEnabled);
        }
    }
    public void DisablePhysics()
    {
        ChildPhysicsEnabled = false;
        foreach(var node in _characterLayer.GetChildren()) node.SetPhysicsProcess(false);
    }

    public void AddPlayerComponents(Character character)
    {
        if (_characters.TryGetValue(character.ID, out var tuple)) 
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

    public void RenderProjectile(Vector3 start, Vector3 end)
    {
        AddChild(new BulletRay3D(start, end));
    }
}