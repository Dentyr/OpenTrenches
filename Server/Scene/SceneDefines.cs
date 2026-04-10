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


        /// <summary>
        /// Mask for bullets shot at ground level
        /// </summary>
        public const uint BulletMaskGround = GroundObjectLayer | BarrierLayer;
        /// <summary>
        /// Mask for bullets shot inside a trench
        /// </summary>
        public const uint BulletMaskTrench = TrenchObjectLayer | GroundTileLayer | BarrierLayer;

        public static uint GetMask(WorldLayer layer) => layer == WorldLayer.Ground ? BulletMaskGround : BulletMaskTrench;
        


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