using Godot;
namespace OpenTrenches.Server.Scene;

public static class SceneDefines
{

    public static class Map
    {
        public const uint NilLayer = 0;
        public const uint BulletLayer = (uint)1 << 0;
        public const uint CharacterLayer = (uint)1 << 1;

        public const uint AllMask = 0xFFFFFFFF;
    }

    public static class PhysicsKey
    {
        public const string Rid = "rid";
        public const string Collider = "collider";

        public const string Shape = "shape";
    }

    public static class Physics
    {
        public static readonly Vector3 g = new Vector3(x:0, y:-9.8f, z:0);
    }
}