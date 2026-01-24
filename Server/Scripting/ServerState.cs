
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using OpenTrenches.Common.Contracts.DTO;
using OpenTrenches.Common.World;
using OpenTrenches.Server.Scripting.Player;

public class ServerState
{
    //* State

    //* chunks
    public ChunkArray2D Chunks { get; } = new();
    
    //* characters
    private readonly Dictionary<ushort, Character> _characters = [];
    public IReadOnlyDictionary<ushort, Character> Characters => _characters;
    public event Action<Character>? CharacterAddedEvent; 
    private void AddCharacter(Character Character)
    {
        if (_characters.TryAdd(Character.ID, Character)) CharacterAddedEvent?.Invoke(Character);
    }

    //* creation

    private ushort _charId = 0;
    public Character CreateCharacter()
    {
        var character = new Character(_charId ++);
        character.FireEvent += HandleFire;
        AddCharacter(character);
        return character;
    }
    //* communication

    private List<AbstractCommandDTO> _commandQueue = [];


    private void HandleFire(Character character, Vector3 target)
    {
        _commandQueue.Add(new ProjectileNotificationCommand(character.Position, target));
    }

    public IEnumerable<AbstractCommandDTO> FlushEvente()
    {
        var temp = _commandQueue;
        _commandQueue = [];
        return temp;
    }
}