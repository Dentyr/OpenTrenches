using Godot;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Resources;
using OpenTrenches.Common.Scene;
using OpenTrenches.Common.World;
using OpenTrenches.Core.Scripting;
using OpenTrenches.Core.Scripting.Graphics;
using OpenTrenches.Core.Scripting.World;

namespace OpenTrenches.Core.Scene.World;

public partial class StructureRenderer : StaticBody2D
{
    private ClientStructure _structure { get; }

    public StructureRenderer(ClientStructure Structure, IClientState ClientState)
    {
        _structure = Structure;

        // Position is at the center of the designated cell.
        Position = ((Vector2)Structure.Position + new Vector2(0.5f, 0.5f)) * CommonDefines.CellSize;

        StructureType type = StructureTypes.Get(Structure.Enum);

        Sprite2D sprite = new()
        {
            Position = (Vector2)type.Profile.GetCenter() * CommonDefines.CellSize,
            Texture = TextureLibrary2D.Structure.Camp,
            Modulate = TeamModulate.GetColor(Structure.Team == ClientState.PlayerCharacter?.Team)
        };
        AddChild(sprite);

        AddChild(new CollisionShape2D()
        {
            Position = (Vector2)type.Profile.GetCenter() * CommonDefines.CellSize,
            Shape = new RectangleShape2D()
            {
                Size = ((Vector2)type.Profile.Size) * CommonDefines.CellSize,
            },
        });

        CollisionLayer = PhysicsDefines.Map.StructureLayer;
        CollisionMask = PhysicsDefines.Map.NilLayer;
    }
}
