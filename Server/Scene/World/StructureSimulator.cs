using Godot;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.World;
using OpenTrenches.Server.Scripting.World;

namespace OpenTrenches.Server.Scene.World;

public partial class StructureSimulator : StaticBody2D
{
    public readonly ServerStructure Structure;

    public StructureSimulator(ServerStructure Structure)
    {
        this.Structure = Structure;

        Position = ((Vector2)Structure.Position) * CommonDefines.CellSize;
        StructureType type = StructureTypes.Get(Structure.Enum);

        AddChild(new CollisionShape2D() 
            {
                Position = ((Vector2)type.Profile.Position) * CommonDefines.CellSize,
                Shape = new RectangleShape2D()
                {
                    Size = ((Vector2)type.Profile.Size) * CommonDefines.CellSize
                }
            }
        );
    }

    
}