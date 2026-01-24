using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using OpenTrenches.Server.Scripting.Player;

namespace OpenTrenches.Server.Scene.World;

public partial class WorldNode : Node3D
{
    //* Characters
    private Dictionary<ushort, CharacterSimulator> _characters = [];
    private Node3D CharacterLayer { get; }

    //* gridmap
    private ChunkLayer ChunkLayer { get; } 

    public WorldNode()
    {
        CharacterLayer = new()
        {
            Name = "Characters",
        };
        AddChild(CharacterLayer);

        ChunkLayer = new()
        {
            Name = "Chunks",
        };
        AddChild(ChunkLayer);
    }


    public void LoadState(ServerState serverState)
    {
        //* cleaning

        foreach (CharacterSimulator simulator in _characters.Values) simulator.QueueFree();
        _characters.Clear();

        //* loading

        ChunkLayer.SetChunks(serverState.Chunks);
        foreach (Character character in serverState.Characters.Values) AddCharacter(character); 
    }

    //* handle game state updates

    public void AddCharacter(Character character)
    {
        if (_characters.TryAdd(character.ID, new CharacterSimulator(character)))
        {
            CharacterSimulator node = _characters[character.ID];
            CharacterLayer.AddChild(node);
        }
    }

}