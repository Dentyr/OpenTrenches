using System.Collections.Generic;
using Godot;
using OpenTrenches.Server.Scripting.World;

namespace OpenTrenches.Server.Scripting.Player;

public abstract record FireHitResult(Vector2 Position)
{
    public record HitStructure(Vector2 Position, ServerStructure Structure) : FireHitResult(Position);
    public record HitCharacter(Vector2 Position, Character Character) : FireHitResult(Position);
    public record Block(Vector2 Position) : FireHitResult(Position);
    public record Miss(Vector2 Position) : FireHitResult(Position);
}

public abstract record WorldQuery
{
    /// <summary>
    /// Finds very close enemies that can or will quickly reach the character
    /// </summary>
    public record MeeleeThreats : WorldQuery;
    /// <summary>
    /// objects facing <paramref name="direction"/>
    /// </summary>
    public record RangeForward(Vector2 direction) : WorldQuery;


    /// <summary>
    /// Finds moderately nearby enemies that comfortably shoot at the character.
    /// </summary>
    public record Threats : WorldQuery;
}

public abstract record WorldQueryResult
{
    public record Found(
        List<Character> Characters,
        List<ServerStructure> Structures
    ) : WorldQueryResult;
}