using System.Collections.Generic;
using Godot;
using OpenTrenches.Common.Factions;
using OpenTrenches.Server.Scripting.Player;

namespace OpenTrenches.Server.Scripting.Teams;

public class Team(int ID, FactionEnum Faction, Vector3 SpawnPoint)
{
    //* identification

    public int ID { get; } = ID;
    public FactionEnum Faction { get; } = Faction;

    
    //* characters

    private readonly List<Character> _characters = [];
    public IReadOnlyList<Character> Characters => _characters;

    public int CharcaterCount => Characters.Count;

    public void AddCharacter(Character character)
    {
        if (character.Team == this) _characters.Add(character);
    }

    public bool RemoveCharacter(Character character) => _characters.Remove(character);

    public Vector3 SpawnPoint { get; } = SpawnPoint;
}