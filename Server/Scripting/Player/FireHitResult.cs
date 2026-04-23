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
