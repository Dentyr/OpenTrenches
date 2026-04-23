using System.Collections.Generic;
using OpenTrenches.Server.Scripting.World;

namespace OpenTrenches.Server.Scripting.Player;

public abstract record WorldQueryResult
{
    public record Found(
        List<Character> Characters,
        List<ServerStructure> Structures
    ) : WorldQueryResult;
}