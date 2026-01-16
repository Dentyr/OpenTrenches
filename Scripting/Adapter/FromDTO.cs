using OpenTrenches.Common.Contracts.DTO;
using OpenTrenches.Core.Scripting.Player;

namespace OpenTrenches.Core.Scripting.Adapter;

public static class FromDTO
{
    public static Character Convert(CharacterDTO dTO)
    {
        return new Character(dTO.ID, dTO.Position, dTO.Health);
    }
}