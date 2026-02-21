using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using OpenTrenches.Common.Contracts.DTO.UpdateModel;
using OpenTrenches.Server.Scripting.Player;

namespace OpenTrenches.Server.Scene.World;

public partial class WorldNode : Node3D, IWorldSimulator
{
    //* Characters
    private Dictionary<ushort, CharacterSimulator> _characters = [];
    private Node3D CharacterLayer { get; }

    //* gridmap
    private ServerChunkLayer? ChunkLayer { get; set; } 

    public WorldNode(ServerState serverState)
    {
        CharacterLayer = new()
        {
            Name = "Characters",
        };
        AddChild(CharacterLayer);

        ChunkLayer = new(serverState.Chunks)
        {
            Name = "Chunks",
        };
        AddChild(ChunkLayer);

        
        foreach (Character character in serverState.Characters.Values) AddCharacter(character); 
    }

    //* handle game state updates

    public void AddCharacter(Character character)
    {
        if (_characters.TryAdd(character.ID, new CharacterSimulator(character, this)))
        {
            CharacterSimulator node = _characters[character.ID];
            CharacterLayer.AddChild(node);
        }
    }

    bool IWorldSimulator.Build(Vector2I cell, TileType buildTarget, float progress) => ChunkLayer?.Build(cell, buildTarget, progress) ?? false;
}

public interface IWorldSimulator
{
    /// <summary>
    /// Attempts to progress the building of <paramref name="buildTarget"/> at <paramref name="cell"/> by <paramref name="progress"/>, returning true if transaction was completed or false otherwise.
    /// </summary>
    bool Build(Vector2I cell, TileType buildTarget, float progress);
}