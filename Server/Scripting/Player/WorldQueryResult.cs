using System.Collections.Generic;
using OpenTrenches.Server.Scripting.World;

namespace OpenTrenches.Server.Scripting.Player;

public class WorldQueryResult
{

    public IReadOnlyList<Character> Characters { get; init; }
    public IReadOnlyList<ServerStructure> Structures { get; init; }

    public WorldQueryResult(IReadOnlyList<Character> Characters, 
        IReadOnlyList<ServerStructure> Structures)
    {
        this.Characters = Characters;
        this.Structures = Structures;
    }
}