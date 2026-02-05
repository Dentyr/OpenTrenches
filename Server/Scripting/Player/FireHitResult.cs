using Godot;

namespace OpenTrenches.Server.Scripting.Player;

public abstract record FireHitResult(Vector3 Position)
{
    public record Hit(Vector3 Position, Character Character) : FireHitResult(Position);
    public record Block(Vector3 Position) : FireHitResult(Position);
    public record Miss(Vector3 Position) : FireHitResult(Position);
}