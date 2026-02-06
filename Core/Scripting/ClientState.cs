
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
using OpenTrenches.Common.Contracts.DTO.PlayerCommands;

namespace OpenTrenches.Core.Scripting;

public sealed class ClientState
{
    private Dictionary<uint, Character> _characters = [];
    public IReadOnlyDictionary<uint, Character> Characters => _characters;

    public event Action<Character>? CharacterAddedEvent; 

    private void AddCharacter(Character Character)
    {
        if (_characters.TryAdd(Character.ID, Character)) CharacterAddedEvent?.Invoke(Character);
    }


    //* 
    public ChunkArray2D Chunks { get; } = new(); //TODO send required size in create message



    //* Events
    public event Action<Character>? PlayerCharacterSetEvent;

    public event Action<Vector3, Vector3>? FireEvent;
    
    public void Update(AbstractUpdateDTO update)
    {
        if (update is CharacterUpdateDTO characterUpdateDTO) if (Characters.TryGetValue(characterUpdateDTO.TargetId, out var character)) character.Update(characterUpdateDTO);
    }
    public void Create(AbstractCreateDTO dTO)
    {
        if (dTO is CharacterDTO character) AddCharacter(FromDTO.Convert(character));
        else if (dTO is WorldChunkDTO chunk) Chunks.SetChunk(CommonFromDTO.Convert(chunk));
        // else if (dTO is WorldChunkDTO chunk) {
        //     Chunks.SetChunk(CommonFromDTO.Convert(chunk));
        // }
    }
    public void Receive(AbstractCommandDTO dto)
    {
        if (dto is SetPlayerCommandDTO setPlayerCommand) 
        {
            if (Characters.TryGetValue(setPlayerCommand.PlayerID, out var character)) PlayerCharacterSetEvent?.Invoke(character);
            else throw new NotImplementedException("Request another ID not implemented");
        }
        else if (dto is SetCellCommand setCell)
        {
            Chunks.Execute(setCell);
        }
        else if (dto is ProjectileNotificationCommand projectile)
        {
            FireEvent?.Invoke(projectile.Start, projectile.End);
        }
        else if (dto is AbilityNotificationCommand abilityNotify)
        {
            if (_characters.TryGetValue(abilityNotify.Character, out var chara))
            {
                chara.ActivateAbility(abilityNotify.Idx);
            }
        }
    }
}