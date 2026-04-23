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

//TODO make modular like godot physics queries
public record class WorldQuery
{
    public virtual bool IncludeAlly() => true;
    public virtual bool IncludeEnemy() => true;
    public virtual bool IncludeStructure() => true;
    public virtual bool IncludeCharacter() => true;

    /// <summary>
    /// Finds very close characters that can quickly reach the character
    /// </summary>
    public record MeeleeThreats : WorldQuery
    {
        public override bool IncludeAlly() => false;
    }
    /// <summary>
    /// objects facing <paramref name="direction"/>
    /// </summary>
    public record RangeForward(Vector2 direction) : WorldQuery;


    /// <summary>
    /// Finds moderately nearby enemies that comfortably shoot at the character.
    /// </summary>
    public record Threats : WorldQuery
    {
        public override bool IncludeAlly() => false;
    }
}

public abstract record WorldQueryResult
{
    public record Found(
        List<Character> Characters,
        List<ServerStructure> Structures
    ) : WorldQueryResult;
}