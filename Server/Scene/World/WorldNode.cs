using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using OpenTrenches.Common.Contracts.DTO.UpdateModel;
using OpenTrenches.Common.Scene.World;
using OpenTrenches.Server.Scripting.Player;
using OpenTrenches.Server.Scripting.World;

namespace OpenTrenches.Server.Scene.World;

public partial class WorldNode : Node3D
{
    //* Characters
    private Dictionary<ushort, CharacterSimulator> _characters = [];
    private Node3D CharacterLayer { get; }

    //* Structures
    private Dictionary<int, StructureSimulator> _structures = [];
    private Node3D StructureLayer { get; }

    //* gridmap
    private ChunkLayer? ChunkLayer { get; set; } 

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
}