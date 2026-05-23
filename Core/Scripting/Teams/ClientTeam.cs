using System.Collections.Generic;
using OpenTrenches.Common.Factions;
using OpenTrenches.Common.World;
using OpenTrenches.Core.Scripting.World;

namespace OpenTrenches.Core.Scripting.Teams;

public class ClientTeam(int ID, FactionRecord Faction)
{
    public int ID { get; } = ID;
    public FactionRecord Faction { get; } = Faction;

    private List<ClientStructure> _camps = [];
    public IReadOnlyList<ClientStructure> Camps => _camps;

    /// <summary>
    /// Records it in Camps if <paramref name="camp"/> is a camp
    /// </summary>
    public void MarkCamp(ClientStructure camp)
    {
        if (camp.Enum == StructureEnum.Camp)
            _camps.Add(camp);
    }
}