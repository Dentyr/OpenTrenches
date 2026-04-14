using Godot;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Resources;
using OpenTrenches.Common.World;
using OpenTrenches.Core.Scripting.World;

namespace OpenTrenches.Core.Scene.World;

public partial class StructureRenderer : Node2D
{
    private ClientStructure _structure { get; }

    public StructureRenderer(ClientStructure Structure)
    {
        _structure = Structure;

        Position = ((Vector2)Structure.Position) * CommonDefines.CellSize;

        StructureType type = StructureTypes.Get(StructureEnum.Camp);

        Sprite2D sprite = new()
        {
            Position = ((Vector2)type.Profile.GetCenter()) * CommonDefines.CellSize,
            Texture = TextureLibrary2D.Structure.Camp,
        };
        AddChild(sprite);
    }
}