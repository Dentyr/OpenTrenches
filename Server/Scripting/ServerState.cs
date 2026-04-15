
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using OpenTrenches.Common.Collections;
using OpenTrenches.Common.Contracts;
using OpenTrenches.Common.Contracts.DTO;
using OpenTrenches.Common.Contracts.DTO.DataModel;
using OpenTrenches.Common.Contracts.DTO.ServerComands;
using OpenTrenches.Common.Factions;
using OpenTrenches.Common.World;
using OpenTrenches.Server.Scripting.Adapter;
using OpenTrenches.Server.Scripting.Player;
using OpenTrenches.Server.Scripting.Teams;
using OpenTrenches.Server.Scripting.World;

public class ServerState : IServerState
{
    public delegate void GameEndedDelegate(Team? victor);

    //* State

    //* chunks
    private ServerChunkArray _chunks { get; } = new();
    public IServerChunkArray Chunks => _chunks;

    //TODO change ushort to be consistent with other dictionaries
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

    public event Action<ServerStructure>? StructureCreatedEvent;

    public event Action<Team>? TeamDestroyedEvent;

    /// <summary>
    /// 
    /// </summary>
    public event GameEndedDelegate? GameEndedEvent;

    //* creation
    private ushort _charId = 0;
    public Character CreateCharacter()
    {
        Character character = new(this, 
            ID: _charId ++, 
            Team: _teams.MinBy(team => team.Value.CharcaterCount).Value);
        
        var spawnPoint = character.Team.SpawnPoint;
        character.Position = new(
            spawnPoint.X + (Random.Shared.NextSingle() * 10f - 5f),
            spawnPoint.Y + (Random.Shared.NextSingle() * 10f - 5f));
        character.FireEvent += HandleFire;
        character.ActivatedAbilityEvent += (idx) => HandleAbility(character.ID, idx);

        character.DiedEvent += () => _commandQueue.Enqueue(new DeathNotificationCommand(character.ID));
        character.RespawnEvent += () => _commandQueue.Enqueue(new RespawnNotificationCommand(character.ID));

        if (!AddCharacter(character)) throw new Exception("Failed to create new character");
        return character;
    }
    private ushort _teamId = 0;
    private Team CreateTeam(FactionEnum faction, Vector2 spawnpoint)
    {
        Team team = new(_teamId ++, faction, spawnpoint);
        team.DestroyedEvent += () => HandleTeamDestroyed(team);
        _teams.Add(team.ID, team);

        return team;
    }


    public ServerState()
    {
        //* event wiring
        _chunks.NewStructureEvent += HandleStructureCreated;

        //* Initial object state

        CreateTeam(FactionEnum.StandardDebug, new(16, 50));
        CreateTeam(FactionEnum.StandardDebug, new(112, 50));

        for (int i = 0; i < 3; i ++)
        {
            int xoffset = i == 1 ? 20 : 10;

            int x1 = xoffset;
            int x2 = _chunks.CellSizeX - xoffset;

            int y = _chunks.CellSizeY * ((i * 2) + 1) / 6;
            if (!_chunks.TryBuild(new(x1, y), Teams[0], StructureEnum.Camp, out var camp1))
                throw new Exception("Initiializing camp failed at " + x1 + ", " + y);
            if (!_chunks.TryBuild(new(x2, y), Teams[1], StructureEnum.Camp, out var camp2))
                throw new Exception("Initiializing camp failed at " + x2 + ", " + y);

        }
    }

    private void HandleStructureCreated(ServerStructure structure)
    {
        if (_teams.TryGetValue(structure.Id, out Team? team)) team.AddStructure(structure);
        StructureCreatedEvent?.Invoke(structure);
    }
        
    private void HandleTeamDestroyed(Team destroyed)
    {
        TeamDestroyedEvent?.Invoke(destroyed);

        Team? victor = null;
        // If there is only one non-destroyed team, they win. 
        // If somehow there is no one left, game ends with no victor.
        foreach(Team team in Teams.Values)
        {
            if (!team.Destroyed)
            {
                // If there are multiple active teams, nothing happens
                if (victor != null) return; 
                victor = team;
            }
        }
        
        GameEndedEvent?.Invoke(victor);
    }

    //* communication

    private PolledQueue<AbstractCommandDTO> _commandQueue = new();

    private void HandleAbility(uint charaIdx, int abilityIdx)
        => _commandQueue.Enqueue(new AbilityNotificationCommand(charaIdx, abilityIdx));

    private void HandleFire(Character character, Vector2 target) 
        => _commandQueue.Enqueue(new ProjectileNotificationCommand(character.Position, target, character.ID));

    public IEnumerable<AbstractCommandDTO> PollEvents() => _commandQueue.PollItems().Concat(_chunks.PollCellChanges());

    public IEnumerable<AbstractCreateDTO> GetInitDTOs()
        => _characters.Values.Select(ObjectToDTO.Convert).Cast<AbstractCreateDTO>()
            .Concat(_chunks.GetChunks().Select(CommonToDTO.Convert))
            .Concat(Teams.Values.Select(ObjectToDTO.Convert))
            .Concat(_chunks.StructureDict.Values.Select(ObjectToDTO.Convert));
            // .Concat(Chunks.GetChunks().Select(chunk => CommonToDTO.Convert(chunk)));
}

public interface IServerState
{
    public IServerChunkArray Chunks { get; }
}
