using Godot;

namespace OpenTrenches.Server.Scripting.Player;

public abstract record FireHitResult(Vector2 Position)
{
    public record Hit(Vector2 Position, Character Character) : FireHitResult(Position);
    public record Block(Vector2 Position) : FireHitResult(Position);
    public record Miss(Vector2 Position) : FireHitResult(Position);
}