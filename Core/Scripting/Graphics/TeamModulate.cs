using Godot;

namespace OpenTrenches.Core.Scripting.Graphics;

public static class TeamModulate
{
    public static readonly Color TeamColor = new(0.7f, 1f, 0.7f, 1f);
    public static readonly Color EnemyColor = new(1f, 0.7f, 0.7f, 1f);

    public static Color GetColor(bool sameTeam)
    {
        return sameTeam ? TeamColor : EnemyColor;
    }
}