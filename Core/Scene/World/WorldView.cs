using System.Collections.Generic;
using Godot;
using OpenTrenches.Core.Scripting.Player;
using OpenTrenches.Core.Scene.Combat;
using OpenTrenches.Common.World;
using OpenTrenches.Server.Scene.World;
using OpenTrenches.Core.Scripting;
using OpenTrenches.Common.Scene.World;

namespace OpenTrenches.Core.Scene.World;

public partial class WorldView : Node2D
{
    private readonly IClientState _clientState;
    //* Characters
    private readonly Dictionary<ushort, CharacterNodesRecord> _characters = [];
    private Node2D _characterLayer { get; }

    //* tiles
    private ChunkLayer ChunkLayer { get; set; } = null!;

    //* UI floats
    private Node CharacterUILayer { get; }

    //* environment settings
    private WorldEnvironment WorldEnvironment { get; }
    

    private bool ChildPhysicsEnabled { get; set; } = true;

    public WorldView(ClientState State)
    {
        _clientState = State;
        
        ChunkLayer = new(State.Chunks, TileSetLibrary.GrassTileSet);
        AddChild(ChunkLayer);

        
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



        //* Load from state
        foreach(var chara in State.Characters.Values) AddCharacter(chara);
    }



    public void AddCharacter(Character character)
    {
        if (_characters.TryAdd(character.ID, new(_clientState, character)))
        {
            CharacterRenderer node = _characters[character.ID].CharacterNode;
            _characterLayer.AddChild(node);

            
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
        if (_characters.TryGetValue(character.ID, out var record)) 
        {
            record.CharacterNode.AddChild(new FocusCamera());
        }

    }

    public override void _Process(double delta)
    {

        //* ui elements

        foreach(var tuple in _characters.Values) 
        {
            tuple.Label.Position = new (tuple.CharacterNode.Position.X, tuple.CharacterNode.Position.Y);
            // tuple.Label.Position -= ne
        }
    }

    public void RenderProjectile(Vector2 start, Vector2 end)
    {
        AddChild(new BulletRay2D(start, end));
    }
}

public class CharacterNodesRecord
{
    public CharacterRenderer CharacterNode { get; }
    public CharacterFloat Label { get; }

    public CharacterNodesRecord(IClientState clientState, Character character)
    {
        CharacterNode = new CharacterRenderer(clientState, character);
        Label = new CharacterFloat(character);

        character.InactivatedEvent += Deactivate;
        character.ActivatedEvent += Activate;

        if (character.IsActive) Activate();
        else Deactivate();
    }

    private void Deactivate()
    {
        CharacterNode.Hide();
        Label.Hide();
    }
    private void Activate()
    {
        CharacterNode.Show();
        Label.Show();
    }
}
