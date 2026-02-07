
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using OpenTrenches.Common.Collections;
using OpenTrenches.Common.Contracts;
using OpenTrenches.Common.Contracts.DTO;
using OpenTrenches.Common.Contracts.DTO.ServerComands;
using OpenTrenches.Common.Factions;
using OpenTrenches.Common.World;
using OpenTrenches.Server.Scripting.Adapter;
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

    private bool AddCharacter(Character Character)
    {
        if (_characters.TryAdd(Character.ID, Character)) 
        {
            Character.Team.AddCharacter(Character);
            CharacterAddedEvent?.Invoke(Character);
            return true;
        }
        return false;
    }

    private readonly Dictionary<int, Team> _teams = [];
    public IReadOnlyDictionary<int, Team> Teams => _teams;


    public event Action<Character>? CharacterAddedEvent; 

    //* creation

    private ushort _charId = 0;
    public Character CreateCharacter()
    {
        Character character = new(this, 
            ID: _charId ++, 
            Team: _teams.MinBy(team => team.Value.CharcaterCount).Value);
        
        character.Position = character.Team.SpawnPoint;
        character.FireEvent += HandleFire;
        character.ActivatedAbilityEvent += (idx) => HandleAbility(character.ID, idx);

        character.DiedEvent += () => _commandQueue.Enqueue(new DeathNotificationCommand(character.ID));
        character.DiedEvent += () => Console.WriteLine("Chara died");
        character.RespawnEvent += () => _commandQueue.Enqueue(new RespawnNotificationCommand(character.ID));

        if (!AddCharacter(character)) throw new Exception("Failed to create new character");
        return character;
    }
    private ushort _teamId = 0;
    private Team CreateTeam(FactionEnum faction, Vector3 spawnpoint)
    {
        Team team = new(_teamId ++, faction, spawnpoint);
        _teams.Add(team.ID, team);

        return team;
    }

    public ServerState()
    {
        CreateTeam(FactionEnum.StandardDebug, new(20, 10, 20));
        CreateTeam(FactionEnum.StandardDebug, new(30, 10, 30));
    }

    //* communication

    private PolledQueue<AbstractCommandDTO> _commandQueue = new();

    private void HandleAbility(uint charaIdx, int abilityIdx)
        => _commandQueue.Enqueue(new AbilityNotificationCommand(charaIdx, abilityIdx));

    private void HandleFire(Character character, Vector3 target) 
        => _commandQueue.Enqueue(new ProjectileNotificationCommand(character.Position, target));

    public IEnumerable<AbstractCommandDTO> PollEvents() => _commandQueue.PollItems().Concat(Chunks.PollCellChanges());

    public IEnumerable<AbstractCreateDTO> GetInitDTOs()
        => _characters.Values.Select(ObjectToDTO.Convert).Cast<AbstractCreateDTO>()
            .Concat(Chunks.GetChunks().Select(CommonToDTO.Convert))
            .Concat(Teams.Values.Select(ObjectToDTO.Convert));
            // .Concat(Chunks.GetChunks().Select(chunk => CommonToDTO.Convert(chunk)));
}

public interface IServerState
{
    public IChunkArray2D Chunks { get; }
}
