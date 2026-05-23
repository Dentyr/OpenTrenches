using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using OpenTrenches.Common.Factions;
using OpenTrenches.Common.World;
using OpenTrenches.Server.Scripting.Player;
using OpenTrenches.Server.Scripting.World;

namespace OpenTrenches.Server.Scripting.Teams;

public class Team(int ID, FactionEnum Faction, Vector2 SpawnPoint)
{
    //* identification

    public int ID { get; } = ID;
    public FactionEnum Faction { get; } = Faction;

    
    //* characters

    private readonly List<Character> _characters = [];
    public IReadOnlyList<Character> Characters => _characters;

    private readonly List<ServerStructure> _camps = [];
    public IReadOnlyList<ServerStructure> Camps => _camps;
    private readonly List<ServerStructure> _structures = [];

    /// <returns>True if all camps are destroyed, false if there is any non-destroyed camp</returns>
    public bool Destroyed => _camps.All(structure => structure.Destroyed);

    //*

    /// <summary>
    /// When the last <see cref="StructureEnum.Camp"/> is destroyed
    /// </summary>
    public event Action? DestroyedEvent;

    public void AddStructure(ServerStructure structure)
    {
        if (structure.Enum == StructureEnum.Camp) 
        {
            structure.DestroyedEvent += HandleCampDestroyed;
            _camps.Add(structure);
        }
        else _structures.Add(structure);
    }
    private void HandleCampDestroyed()
    {
        if (Destroyed) DestroyedEvent?.Invoke();
    }

    public int CharcaterCount => Characters.Count;

    public void AddCharacter(Character character)
    {
        if (character.Team == this) _characters.Add(character);
    }

    public bool RemoveCharacter(Character character) => _characters.Remove(character);

    /// <summary>
    /// Characters will use this spawnpoint when no cmaps are found (for debug)
    /// </summary>
    public Vector2 DefaultSpawnPoint { get; } = SpawnPoint;
}