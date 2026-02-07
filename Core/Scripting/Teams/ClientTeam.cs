using OpenTrenches.Common.Factions;

namespace OpenTrenches.Core.Scripting.Teams;

public class ClientTeam(int ID, FactionRecord Faction)
{
    public int ID { get; } = ID;
    public FactionRecord Faction { get; } = Faction;
}