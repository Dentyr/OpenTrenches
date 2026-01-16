
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
    
    public void Update(ObjectCategory category, ushort id, Update update)
    {
        switch (category)
        {
            case ObjectCategory.Character:
                if (Characters.TryGetValue(id, out var character)) character.Update(update);
                break;
            default:
                break;
        }
    }
    public void Create(ObjectCategory category, AbstractDTO dTO)
    {
        if (dTO is CharacterDTO character) AddCharacter(FromDTO.Convert(character));
        switch (category)
        {
            case ObjectCategory.Character:
                
                break;
            default:
                break;
        }
    }
    public void Receive(MessageCategory messageType, byte[] message)
    {
        switch (messageType)
        {
            case MessageCategory.Setplayer:
                PlayerCharacterSetEvent?.Invoke(Serialization.Deserialize<ushort>(message));
                break;
        }
    }
}