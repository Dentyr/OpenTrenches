using Godot;

namespace OpenTrenches.Common.World;

public class StructureType
{
    /// <summary>
    /// The structure's profile is the rectangle of cells surrounded by begin and end
    /// </summary>
    public Vector2I Begin { get; }
    /// <summary>
    /// The structure's profile is the rectangle of cells surrounded by begin and end
    /// </summary>
    public Vector2I End { get; }

    public StructureType(Vector2I Begin, Vector2I End)
    {
        this.Begin = Begin;
        this.End = End;
    }

    public bool Constructable { get; init; } = true;

    public int HitPoints { get; init; } = -1;
    public bool Destructable => HitPoints > 0;
}
