using System.Collections.Generic;
using Godot;
using OpenTrenches.Core.Scripting.Player;
using OpenTrenches.Core.Scene.Combat;
using OpenTrenches.Common.World;
using OpenTrenches.Server.Scene.World;
using OpenTrenches.Core.Scripting;

namespace OpenTrenches.Core.Scene.World;

public partial class WorldView : Node3D
{
    //* Characters
    private readonly Dictionary<ushort, (CharacterRenderer CharacterNode, CharacterFloat Label)> _characters = [];
    private Node3D _characterLayer { get; }

    //* tiles
    private RenderChunkLayer ChunkLayer { get; set; } = null!;

    //* UI floats
    private Node CharacterUILayer { get; }

    //* environment settings
    private WorldEnvironment WorldEnvironment { get; }
    

    private bool ChildPhysicsEnabled { get; set; } = true;

    public WorldView(ClientState State)
    {
        _characterLayer = new()
        {
            Name = "Characters",
        };
        AddChild(_characterLayer);

        

        CharacterUILayer = new();
        AddChild(CharacterUILayer);

        WorldEnvironment = new()
        {
            Environment = SceneDefines.IlluminatedEnvironment,
        };
        AddChild(WorldEnvironment);


        ChunkLayer = new(State.Chunks);
        AddChild(ChunkLayer);
    }



    public void AddCharacter(Character character)
    {
        if (_characters.TryAdd(character.ID, new(new CharacterRenderer(character), new CharacterFloat(character))))
        {
            CharacterRenderer node = _characters[character.ID].CharacterNode;
            _characterLayer.AddChild(node);

            CharacterFloat label = _characters[character.ID].Label;
            CharacterUILayer.AddChild(label);
            
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