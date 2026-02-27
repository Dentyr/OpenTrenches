
using System;
using System.Collections.Generic;
using OpenTrenches.Common.Multiplayer;
using OpenTrenches.Core.Scripting.Player;
using OpenTrenches.Common.Contracts;
using OpenTrenches.Common.Contracts.DTO;
using OpenTrenches.Core.Scripting.Adapter;
using Godot;
using OpenTrenches.Core.Scene;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.World;
using OpenTrenches.Common.Contracts.DTO.DataModel;
using OpenTrenches.Core.Scripting.Teams;
using OpenTrenches.Common.Contracts.DTO.ServerComands;
using OpenTrenches.Common.Contracts.DTO.UpdateModel;
using System.Diagnostics.CodeAnalysis;

namespace OpenTrenches.Core.Scripting;

public sealed class ClientState : IClientState
{
    //* Status
    public bool Loaded { get; private set; }
    public event Action? LoadedEvent;
    private void SetLoaded()
    {
        Loaded = true;
        LoadedEvent?.Invoke();
    }

    //* Player events ( for rendering )

    public event Action? PlayerDeathEvent;

    public event Action? PlayerRespawnEvent;

    public event Action? PlayerReloadEvent;
    public event Action? PlayerFireEvent;

    //* State

    //* character
    private readonly Dictionary<uint, Character> _characters = [];
    public IReadOnlyDictionary<uint, Character> Characters => _characters;
    public bool TryGetCharacter(uint id, [NotNullWhen(true)] out Character? character) => Characters.TryGetValue(id, out character);

    public event Action<Character>? CharacterAddedEvent;

    private void AddCharacter(Character Character)
    {
        if (_characters.TryAdd(Character.ID, Character)) CharacterAddedEvent?.Invoke(Character);
    }

    //* player info
    private PlayerState _playerState = new() { Logistics = 12 }; //TODO test value
    public IReadOnlyPlayerState PlayerState => _playerState;


    public uint? PlayerCharacterId { get; private set; }
    public Character? PlayerCharacter {
        get {
            if (PlayerCharacterId is uint notnull && TryGetCharacter(notnull, out var character)) return character;
            return null;
        }
    }

    //* 
    public ChunkArray2D Chunks { get; } = new(); //TODO send required size in create message


    private readonly Dictionary<int, ClientTeam> _teams = [];
    public IReadOnlyDictionary<int, ClientTeam> Team => _teams;

    private void AddTeam(ClientTeam team)
    {
        _teams.Add(team.ID, team);
    }


    //* Events
    public event Action<LocalPlayerView>? PlayerCharacterSetEvent;

    public event Action<Vector3, Vector3>? FireEvent;

    //TODO switch to dictionary approach
    public void Update(AbstractUpdateDTO update)
    {
        if (update is CharacterUpdateDTO characterUpdateDTO)
        {
            if (Characters.TryGetValue(characterUpdateDTO.TargetId, out var character)) character.Update(characterUpdateDTO);
        }
        else if (update is FirearmSlotUpdateDTO firearmUpdateDTO)
        {
            _playerState.Update(firearmUpdateDTO);
        }
        else if (update is PlayerUpdateDTO playerUpdateDTO)
        {
            _playerState.Update(playerUpdateDTO);
        }
    }
    public void Create(AbstractCreateDTO dTO)
    {
        if (dTO is CharacterDTO character) AddCharacter(FromDTO.Convert(character, this));
        else if (dTO is WorldChunkDTO chunk) Chunks.SetChunk(CommonFromDTO.Convert(chunk));
        else if (dTO is TeamDTO team) AddTeam(FromDTO.Convert(team));
        // else if (dTO is WorldChunkDTO chunk) {
        //     Chunks.SetChunk(CommonFromDTO.Convert(chunk));
        // }
    }
    public void Receive(AbstractCommandDTO dto)
    {
        if (dto is SetPlayerCommandDTO setPlayerCommand)
        {
            if (Characters.TryGetValue(setPlayerCommand.PlayerID, out var character))
            {
                if (PlayerCharacterId is not null) Console.Error.WriteLine("Attempted to re-set player");

                PlayerCharacterId = setPlayerCommand.PlayerID;
                character.InactivatedEvent += () => PlayerDeathEvent?.Invoke();
                character.ActivatedEvent += () => PlayerRespawnEvent?.Invoke();

                //* state
                _playerState = new();
                _playerState.PrimarySlotState.AmmoLoaded = setPlayerCommand.AmmoLoaded;
                _playerState.PrimarySlotState.AmmoStored = setPlayerCommand.AmmoStored;

                PlayerCharacterSetEvent?.Invoke(new LocalPlayerView(character, _playerState));
            }
            else throw new NotImplementedException("Set player failed; re-request not implemented");
        }
        else if (dto is SetCellCommand setCell)
        {
            Chunks.Execute(setCell);
        }
        else if (dto is ProjectileNotificationCommand projectile)
        {
            if (projectile.Character == PlayerCharacterId) PlayerFireEvent?.Invoke();
            FireEvent?.Invoke(projectile.Start, projectile.End);
        }
        else if (dto is AbilityNotificationCommand abilityNotify)
        {
            if (_characters.TryGetValue(abilityNotify.Character, out var chara))
            {
                chara.ActivateAbility(abilityNotify.Idx);
            }
        }
        else if (dto is DeathNotificationCommand characterDeath)
        {
            if (Characters.TryGetValue(characterDeath.Character, out Character? character))
            {
                //TODO implement log?
            }
        }
        else if (dto is RespawnNotificationCommand characterRespawn)
        {
            if (Characters.TryGetValue(characterRespawn.Character, out Character? character))
            {
                //TODO implement log?
            }
        }
        else if (dto is InitializedNotificationCommand) SetLoaded();
        else if (dto is ReloadNotificationCommand)
        {
            PlayerReloadEvent?.Invoke();
        }
    }
}

public record class LocalPlayerView
{
    public Character Character { get; }
    public IReadOnlyPlayerState PlayerState { get; }

    public LocalPlayerView(Character Character, IReadOnlyPlayerState PlayerState)
    {
        this.Character = Character;
        this.PlayerState = PlayerState;
    }
}

public interface IClientState
{
    public IReadOnlyDictionary<uint, Character> Characters { get; }
    public IReadOnlyDictionary<int, ClientTeam> Team { get; }
    public Character? PlayerCharacter { get; }
    public IReadOnlyPlayerState PlayerState { get; }
}