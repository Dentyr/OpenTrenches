using Godot;

namespace OpenTrenches.Common.Resources;

public static class Meshes
{
    public static class Terrain
    {
        public static MeshLibrary Library = (MeshLibrary)ResourceLoader.Singleton.Load("Common/Mesh/Libraries/TerrainLibrary.tres");
        public const int GrassIdx = 0;
        public const int TrenchIdx = 1;
    }

    public static Mesh Dirt = (Mesh)ResourceLoader.Singleton.Load("Common/Mesh/Dirt.res");
    // public static Mesh Dirt = TerrainLibrary.GetItemMesh(0);
    // public static Mesh Trench = TerrainLibrary.GetItemMesh(1);
}