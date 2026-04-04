using Godot;
namespace OpenTrenches.Server.Scene;

public static class SceneDefines
{

    public static class Map
    {
        public const uint NilLayer = 0;
        public const uint BulletLayer = (uint)1 << 0;
        public const uint CharacterLayer = (uint)1 << 1;
        public const uint TerrainLayer = (uint)1 << 2;


        public const uint AllMask = 0xFFFFFFFF;

        public const uint BulletMask = CharacterLayer | TerrainLayer;

    }

    public static class PhysicsKey
    {
        public const string Rid = "rid";
        public const string Collider = "collider";

        public const string Shape = "shape";

        public const string Position = "position";
    }
}