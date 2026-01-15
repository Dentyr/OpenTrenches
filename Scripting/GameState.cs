
using System;
using System.Collections.Generic;
using OpenTrenches.Scripting.Datastream;
using OpenTrenches.Scripting.Multiplayer;
using OpenTrenches.Scripting.Player;

public class GameState
{
    private ushort _charId = 0;
    private Dictionary<ushort, Character> _characters = [];
    public IReadOnlyDictionary<ushort, Character> Characters => _characters;

    public event Action<ushort, Character>? CharacterAddedEvent; 

    public ushort CreateCharacter(Character Character)
    {
        ushort id = _charId ++;
        _characters.Add(id, Character);
        CharacterAddedEvent?.Invoke(id, Character);
        return id;
    }
    private void AddCharacter(ushort id, Character Character)
    {
        if (_characters.TryAdd(id, Character)) CharacterAddedEvent?.Invoke(id, Character);
    }

    public void Update(ObjectCategory category, ushort id, Update update)
    {
        switch (category)
        {
            case ObjectCategory.Character:
                if (_characters.TryGetValue(id, out var character)) character.Update(update);
                break;
            default:
                break;
        }
    }
    public void Create(ObjectCategory category, ushort id, ReadOnlyMemory<byte> bytes)
    {
        switch (category)
        {
            case ObjectCategory.Character:
                AddCharacter(id, Serialization.Deserialize<Character>(bytes));
                break;
            default:
                break;
        }
    }
}