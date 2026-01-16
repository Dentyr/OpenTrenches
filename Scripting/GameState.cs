
using System;
using System.Collections.Generic;
using OpenTrenches.Common.Multiplayer;
using OpenTrenches.Core.Scripting.Player;
using OpenTrenches.Common.Contracts;
using OpenTrenches.Common.Contracts.DTO;
using OpenTrenches.Core.Scripting.Adapter;

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
    public event Action<ushort>? PlayerCharacterSetEvent;
    
    public void Update(AbstractUpdateDTO update)
    {
        if (update is CharacterUpdateDTO characterUpdateDTO) if (Characters.TryGetValue(characterUpdateDTO.TargetId, out var character)) character.Update(characterUpdateDTO);
    }
    public void Create(AbstractDTO dTO)
    {
        if (dTO is CharacterDTO character) AddCharacter(FromDTO.Convert(character));
    }
    public void Receive(AbstractCommandDTO dto)
    {
        if (dto is SetPlayerCommandDTO setPlayerCommand) PlayerCharacterSetEvent?.Invoke(setPlayerCommand.PlayerID);
    }
}