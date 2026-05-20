using Godot;
using OpenTrenches.Common.Combat;
using OpenTrenches.Common.Scene;
using OpenTrenches.Common.World;
namespace OpenTrenches.Server.Scene;

public static class SceneDefines
{

    public static class Map
    {
        public const uint NilLayer = PhysicsDefines.Map.NilLayer;
        public const uint BulletLayer = PhysicsDefines.Map.BulletLayer;
        public const uint CharacterLayer = PhysicsDefines.Map.CharacterLayer;

        public const uint StructureLayer = PhysicsDefines.Map.StructureLayer;


        /// <summary>
        /// Collision layer for trench tiles
        /// </summary>
        public const uint TrenchTileLayer = PhysicsDefines.Map.TrenchTileLayer;
        /// <summary>
        /// Collision layer for ground tiles
        /// </summary>
        public const uint GroundTileLayer = PhysicsDefines.Map.GroundTileLayer;
        /// <summary>
        /// Inaccessible area
        /// </summary>
        public const uint BarrierLayer = PhysicsDefines.Map.BarrierLayer;



        


        public const uint AllMask = PhysicsDefines.Map.AllMask;

        public const uint BulletMask = PhysicsDefines.Map.BulletMask;

    }

    public static class PhysicsKey
    {
        public const string Rid = PhysicsDefines.PhysicsKey.Rid;
        public const string Collider = PhysicsDefines.PhysicsKey.Collider;

        public const string Shape = PhysicsDefines.PhysicsKey.Shape;
        public const string Normal = PhysicsDefines.PhysicsKey.Normal;

        public const string Position = PhysicsDefines.PhysicsKey.Position;
    }
}
