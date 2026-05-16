using Godot;
using OpenTrenches.Server.Scripting.Teams;

namespace OpenTrenches.Server.Scene.World;

public static class DebugColoring
{
    public static Color GetColor(Team team)
    {
        if (team.ID % 2 == 0)
            return Colors.Red;
        else
            return Colors.Blue;
    }
}