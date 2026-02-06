
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using OpenTrenches.Common.Collections;
using OpenTrenches.Common.Contracts.DTO;
using OpenTrenches.Common.Contracts.DTO.PlayerCommands;
using OpenTrenches.Common.World;
using OpenTrenches.Server.Scripting.Player;
using OpenTrenches.Server.Scripting.Teams;

public class ServerState : IServerState
{
    //* State

    //* chunks
    public ChunkArray2D Chunks { get; } = new();
    IChunkArray2D IServerState.Chunks => Chunks;
    
    //* characters
    private readonly Dictionary<ushort, Character> _characters = [];
    public IReadOnlyDictionary<ushort, Character> Characters => _characters;

    private void AddCharacter(Character Character)
    {
        if (_characters.TryAdd(Character.ID, Character)) CharacterAddedEvent?.Invoke(Character);
    }

    private readonly Dictionary<int, Team> _teams = [];
    public IReadOnlyDictionary<int, Team> Teams => _teams;


    public event Action<Character>? CharacterAddedEvent; 

    //* creation

    private ushort _charId = 0;
    public Character CreateCharacter()
    {
        var character = new Character(this, _charId ++);
        
        character.FireEvent += HandleFire;
        character.ActivatedAbilityEvent += (idx) => HandleAbility(character.ID, idx);

        AddCharacter(character);
        return character;
    }

    //* communication

    private PolledQueue<AbstractCommandDTO> _commandQueue = new();

    private void HandleAbility(uint charaIdx, int abilityIdx)
        => _commandQueue.Enqueue(new AbilityNotificationCommand(charaIdx, abilityIdx));

    private void HandleFire(Character character, Vector3 target) 
        => _commandQueue.Enqueue(new ProjectileNotificationCommand(character.Position, target));

    public IEnumerable<AbstractCommandDTO> PollEvents() => _commandQueue.PollItems().Concat(Chunks.PollCellChanges());
}

public interface IServerState
{
    public IChunkArray2D Chunks { get; }
}
