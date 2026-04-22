using Godot;
using OpenTrenches.Common.World;
namespace OpenTrenches.Server.Scene;

public static class SceneDefines
{

    public static class Map
    {
        public const uint NilLayer = 0;
        public const uint BulletLayer = (uint)1 << 0;
        public const uint CharacterLayer = (uint)1 << 1;
        /// <summary>
        /// Collision layer for trench tiles
        /// </summary>
        public const uint TrenchTileLayer = (uint)1 << 4;
        /// <summary>
        /// Collision layer for ground tiles
        /// </summary>
        public const uint GroundTileLayer = (uint)1 << 5;
        /// <summary>
        /// Collision layer for objects inside a trench
        /// </summary>
        public const uint TrenchObjectLayer = (uint)1 << 8;
        /// <summary>
        /// Collision layer for objects on the ground
        /// </summary>
        public const uint GroundObjectLayer = (uint)1 << 9;
        /// <summary>
        /// Inaccessible area
        /// </summary>
        public const uint BarrierLayer = (uint)1 << 31;



        public static uint GetBulletMask(WorldLayer layer, bool aiming) 
        {
            // Can only hit ground level objects when on ground level
            if (layer == WorldLayer.Ground) return GroundObjectLayer | BarrierLayer;
            // Can hit characters inside and outside a trench when aiming from trench
            else if (aiming) return TrenchObjectLayer | GroundObjectLayer | BarrierLayer;
            // Can only hit trench objects inside trench and not aiming.
            else return TrenchObjectLayer | GroundTileLayer | BarrierLayer;
        } 
        


        public const uint AllMask = 0xFFFFFFFF;

        public const uint BulletMask = CharacterLayer | GroundTileLayer;

    }

    public static class PhysicsKey
    {
        public const string Rid = "rid";
        public const string Collider = "collider";

        public const string Shape = "shape";
        public const string Normal = "normal";

        public const string Position = "position";
    }
}