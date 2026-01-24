using OpenTrenches.Common.Contracts.DTO;
using OpenTrenches.Server.Scripting.Player;

namespace OpenTrenches.Server.Scripting.Adapter;

public static class ObjectToDTO
{
    public static CharacterDTO Convert(Character character)
        => new (character.ID, character.Movement, character.Health);

}