using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using OpenTrenches.Common.Contracts.DTO.UpdateModel;
using OpenTrenches.Common.World;
using OpenTrenches.Server.Scripting.Player;
using OpenTrenches.Server.Scripting.World;

namespace OpenTrenches.Server.Scene.World;

public partial class WorldNode : Node2D
{
    //* Characters
    private Dictionary<ushort, CharacterSimulator> _characters = [];
    private Node2D CharacterLayer { get; }

    //* Structures
    private Dictionary<int, StructureSimulator> _structures = [];
    private Node2D StructureLayer { get; }

    //* gridmap
    private ServerChunkLayer? ChunkLayer { get; set; } 

    public WorldNode(ServerState serverState)
    {
        ChunkLayer = new(serverState.Chunks)
        {
            Name = "Chunks",
        };
        AddChild(ChunkLayer);
        
        CharacterLayer = new()
        {
            Name = "Characters",
        };
        AddChild(CharacterLayer);

        StructureLayer = new()
        {
            Name = "Structures",
        };
        AddChild(StructureLayer);


        
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

    public void AddStructure(ServerStructure structure)
    {
        if (_structures.TryAdd(structure.Id, new StructureSimulator(structure)))
        {
            StructureSimulator node = _structures[structure.Id];
            StructureLayer.AddChild(node);
        }
    }

    /// <summary>
    /// Creates an object to handle queries against the world
    /// </summary>
    public World2DQueryService CreateQueryService()
    {
        return new World2DQueryService(GetViewport().World2D.DirectSpaceState);
    }
}