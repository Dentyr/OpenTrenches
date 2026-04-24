using Godot;
using OpenTrenches.Common.Combat;
using OpenTrenches.Common.World;
namespace OpenTrenches.Server.Scene;

public static class SceneDefines
{

    public static class Map
    {
        public const uint NilLayer = 0;
        public const uint BulletLayer = (uint)1 << 0;
        public const uint CharacterLayer = (uint)1 << 1;

        public const uint StructureLayer = (uint)1 << 2;


        /// <summary>
        /// Collision layer for trench tiles
        /// </summary>
        public const uint TrenchTileLayer = (uint)1 << 4;
        /// <summary>
        /// Collision layer for ground tiles
        /// </summary>
        public const uint GroundTileLayer = (uint)1 << 5;
        /// <summary>
        /// Inaccessible area
        /// </summary>
        public const uint BarrierLayer = (uint)1 << 31;



        


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