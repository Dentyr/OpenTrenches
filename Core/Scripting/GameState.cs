
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

namespace OpenTrenches.Core.Scripting;
public class GameState
{
    private Dictionary<ushort, Character> _characters = [];
    public IReadOnlyDictionary<ushort, Character> Characters => _characters;

    public event Action<Character>? CharacterAddedEvent; 

    protected void AddCharacter(Character Character)
    {
        if (_characters.TryAdd(Character.ID, Character)) CharacterAddedEvent?.Invoke(Character);
    }

}

public class ClientState : GameState
{
    //* 
    public ChunkArray2D Chunks { get; } = new(); //TODO send required size in create message


    //*
    private void SetChunk(ChunkRecord record)
    {
        Chunks[record.X, record.Y] = record.Chunk;
        ChunkSetEvent?.Invoke(record);
    }

    //* Events
    public event Action<Character>? PlayerCharacterSetEvent;
    public event Action<ChunkRecord>? ChunkSetEvent;

    public event Action<Vector3, Vector3>? FireEvent;
    
    public void Update(AbstractUpdateDTO update)
    {
        if (update is CharacterUpdateDTO characterUpdateDTO) if (Characters.TryGetValue(characterUpdateDTO.TargetId, out var character)) character.Update(characterUpdateDTO);
    }
    public void Create(AbstractDTO dTO)
    {
        if (dTO is CharacterDTO character) AddCharacter(FromDTO.Convert(character));
        else if (dTO is WorldChunkDTO chunk) SetChunk(CommonFromDTO.Convert(chunk));
    }
    public void Receive(AbstractCommandDTO dto)
    {
        if (dto is SetPlayerCommandDTO setPlayerCommand) 
        {
            if (Characters.TryGetValue(setPlayerCommand.PlayerID, out var character)) PlayerCharacterSetEvent?.Invoke(character);
            else throw new NotImplementedException("Request another ID not implemented");
        }
        else if (dto is ProjectileNotificationCommand projectile)
        {
            FireEvent?.Invoke(projectile.Start, projectile.End);
        }
    }
}