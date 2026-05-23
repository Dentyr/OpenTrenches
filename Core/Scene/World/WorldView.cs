using System.Collections.Generic;
using Godot;
using OpenTrenches.Core.Scripting.Player;
using OpenTrenches.Core.Scene.Combat;
using OpenTrenches.Common.World;
using OpenTrenches.Core.Scripting;
using OpenTrenches.Core.Scripting.World;
using OpenTrenches.Common.Scene;

namespace OpenTrenches.Core.Scene.World;

public partial class WorldView : Node2D
{
    private ClientState? _clientState;
    //* Characters
    private readonly Dictionary<int, CharacterNodesRecord> _characters = [];
    private Node2D _characterLayer { get; }

    //* Structure
    private readonly Dictionary<int, StructureRenderer> _structure = [];
    private Node2D _structureLayer { get; }

    //* tiles
    private ClientChunkLayer ChunkLayer { get; set; } = null!;
    

    private bool ChildPhysicsEnabled { get; set; } = true;

    public WorldView()
    {
        
        ChunkLayer = new();
        AddChild(ChunkLayer);

        
        _structureLayer = new()
        {
            Name = "Structures",
        };
        AddChild(_structureLayer);

        _characterLayer = new()
        {
            Name = "Characters",
        };
        AddChild(_characterLayer);
    }

    public void SetClientState(ClientState State)
    {
        //* cleanup
        foreach (var record in _characters.Values) record.CharacterNode.QueueFreeDeferred();
        foreach (var node in _structure.Values) node.QueueFreeDeferred();
        _characters.Clear();
        _structure.Clear();

        _clientState = State;
        ChunkLayer.SetArray(State.Chunks);


        //* Load from state
        foreach(var chara in State.Characters.Values) AddCharacter(chara);
        foreach(var structure in State.Chunks.StructureDict.Values) AddStructure(structure);

        //* events

        State.CharacterAddedEvent += AddCharacter;
        State.StructureAddedEvent += AddStructure;
        State.FireEvent += RenderProjectile;
    }



    public void AddCharacter(Character character)
    {
        if (_clientState is null)
            return;
            
        if (_characters.TryAdd(character.ID, new(_clientState, character)))
        {
            CharacterRenderer node = _characters[character.ID].CharacterNode;
            _characterLayer.AddChild(node);

            
            node.SetPhysicsProcess(ChildPhysicsEnabled);
        }
    }

    public void AddStructure(ClientStructure structure)
    {
        if (_clientState is null)
            return;

        StructureRenderer renderer = new(structure, _clientState);
        _structureLayer.AddChild(renderer);
        renderer.SetPhysicsProcess(ChildPhysicsEnabled);

        _structure[structure.Id] = renderer;
    }
    public void DisablePhysics()
    {
        ChildPhysicsEnabled = false;
        foreach(var node in _characterLayer.GetChildren()) node.SetPhysicsProcess(false);
    }

    public void AddPlayerComponents(Character character, IReadOnlyPlayerState state)
    {
        if (_characters.TryGetValue(character.ID, out var record)) 
        {
            record.CharacterNode.AddChild(new PlayerAimController(state));
        }

    }


    public void RenderProjectile(Vector2 start, Vector2 end)
    {
        AddChild(new BulletRay2D(start, end));
    }
}

/// <summary>
/// Legacy class kept so that UI elements like HP and names can be moved to an alternate layer in the future
/// </summary>
public class CharacterNodesRecord
{
    public CharacterRenderer CharacterNode { get; }

    public CharacterNodesRecord(IClientState clientState, Character character)
    {
        CharacterNode = new CharacterRenderer(clientState, character);

        character.InactivatedEvent += Deactivate;
        character.ActivatedEvent += Activate;

        if (character.IsActive) Activate();
        else Deactivate();
    }

    private void Deactivate()
    {
        CharacterNode.Hide();
    }
    private void Activate()
    {
        CharacterNode.Show();
    }
}
