using OpenTrenches.Common.Combat;
using OpenTrenches.Common.World;
using OpenTrenches.Server.Scripting.Player;

namespace OpenTrenches.Server.Scene.World;

/// <summary>
/// Responsible for converting the game layers of characters like <see cref="WorldLayer"/> into physics layers
/// </summary>
public static class PhysicsLayerInterpreter
{
    /// <summary>
    /// Retursn a mask of physics layers hitscans should scan for a bullet travelling through <paramref name="channel"/>
    /// </summary>
    public static uint GetScanLayer(WorldLayer channel)
    {
        switch (channel)
        {
            case WorldLayer.Ground:
            default:
                return SceneDefines.Map.StructureLayer | SceneDefines.Map.BarrierLayer | SceneDefines.Map.CharacterLayer;
            case WorldLayer.Trench:
                return SceneDefines.Map.StructureLayer | SceneDefines.Map.BarrierLayer | SceneDefines.Map.CharacterLayer | SceneDefines.Map.GroundTileLayer;
        }
    }


    public static uint GetMovementMask(WorldLayer layer)
    {
        // Can only hit ground level objects when on ground level
        if (layer == WorldLayer.Ground) return SceneDefines.Map.BarrierLayer;
        // Can hit characters inside and outside a trench when aiming from trench
        else return SceneDefines.Map.TrenchTileLayer | SceneDefines.Map.BarrierLayer;
    }
}