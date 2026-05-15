using System.Linq;
using Godot;
using OpenTrenches.Common.World;
using OpenTrenches.Server.Scripting.Player;
using OpenTrenches.Server.Scripting.Teams;

namespace OpenTrenches.Server.Scripting.World;

/// <summary>
/// Static class offering services to check the status of areas within a world
/// </summary>
public static class WorldAreaService
{
    
    private static readonly WorldQuery AreaQuery = new()
    {
        QueryArea = WorldQuery.Shape.Area,
        IncludeAlly = true,
        IncludeEnemy = true,
        IncludeCharacter = true,
    };

    /// <summary>
    /// Checks the ratio of friendly to hostile units for <paramref name="team"/> in <paramref name="area"/>
    /// </summary>
    public static Occupation CheckOccupation(Vector2I area, Team team, IWorld2DQueryService service, IServerChunkArray chunkArray)
    {
        var results = service.Query(new QueryContext(AreaTranslationService.GetAreaCenter(area), team), AreaQuery);
        int friendly = results.Characters.Count(chara => chara.Team == team);
        int hostile = results.Characters.Count(chara => chara.Team != team);
        

        if (results.Characters.Count == 0) return Occupation.Neutral;
        // if either friendly or hostile forces overpower the other more than 5:1, count as occupied by them
        else if (friendly > hostile * 5) return Occupation.Friendly;
        else if (hostile > friendly * 5) return Occupation.Hostile;

        return Occupation.Contested;
    }
}

public enum Occupation
{
    /// <summary>
    /// Overwhemingly hostile units
    /// </summary>
    Hostile,
    /// <summary>
    /// There are units, but not enough to conclusively claim occupation
    /// </summary>
    Contested,
    /// <summary>
    /// Overwhemingly friendly units
    /// </summary>
    Friendly,
    /// <summary>
    /// No units
    /// </summary>
    Neutral,
}