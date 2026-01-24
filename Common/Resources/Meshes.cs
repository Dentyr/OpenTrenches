using Godot;

namespace OpenTrenches.Common.Resources;

public static class Meshes
{
    public static MeshLibrary TerrainLibrary = (MeshLibrary)ResourceLoader.Singleton.Load("Common/Mesh/Libraries/TerrainLibrary.tres");

    public static Mesh Dirt = (Mesh)ResourceLoader.Singleton.Load("Common/Mesh/Dirt.res");
}