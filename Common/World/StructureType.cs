using Godot;

namespace OpenTrenches.Common.World;
public class StructureType
{
    public StructureEnum Enum { get; }

    /// <summary>
    /// The space the structure spans
    /// </summary>
    /// <value></value>
    public Rect2I Profile { get; }

    public StructureType(StructureEnum Enum, Rect2I Profile)
    {
        this.Enum = Enum;
        this.Profile = Profile;
    }

    public bool Constructable { get; init; } = true;

    public int HitPoints { get; init; } = -1;
    public bool Destructable => HitPoints > 0;

    /// <summary>
    /// Whether or not this structure will continue to have a profile even if destroyed, as a collapsed structure.
    /// </summary>
    public bool Persistent { get; init; } = false;
}
