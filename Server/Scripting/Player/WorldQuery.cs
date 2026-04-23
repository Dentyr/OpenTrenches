using Godot;

namespace OpenTrenches.Server.Scripting.Player;

public class WorldQuery
{
    public bool IncludeAlly { get; init; } = true;
    public bool IncludeEnemy { get; init; } = true;
    public bool IncludeStructure { get; init; } = true;
    public bool IncludeCharacter { get; init; } = true;

    public Vector2 Direction { get; init; } = Vector2.Zero;

    /// <summary>
    /// The shape of the area which will be queried for matches.
    /// <see cref="Shape.Meelee"/> by default.
    /// </summary>
    /// <value></value>
    public Shape QueryArea { get; init; } = Shape.Meelee;


    public enum Shape
    {
        Meelee,
        /// <summary>
        /// Units close enough to comfortably shoot at
        /// </summary>
        Range,
        /// <summary>
        /// Loner range threats focused on direction
        /// </summary>
        Distance,
    }
}
