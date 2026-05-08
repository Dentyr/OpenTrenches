using System;
using Godot;
using OpenTrenches.Server.Scripting.Player;
using OpenTrenches.Server.Scripting.Teams;

namespace OpenTrenches.Server.Scripting.World;

/// <summary>
/// The context of the object that is creating a world query
/// </summary>
public struct QueryContext(
    Vector2 Position,
    Team Team
) {
    public Vector2 Position { get; } = Position;
    public Team Team { get; } = Team;

    public static QueryContext MakeContext(Character character)
    {
        return new(character.Position, character.Team);
    }
}