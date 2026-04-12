using Godot;

namespace OpenTrenches.Common.World;

public abstract class AbstractStructure
{
    /// <summary>
    /// Cell in the world this structure is placed
    /// </summary>
    public Vector2I Position { get; }

    public StructureType Type { get; }

    

    public AbstractStructure(StructureType Type, Vector2I Position)
    {
        this.Type = Type;
        this.Position = Position;
    }
}
