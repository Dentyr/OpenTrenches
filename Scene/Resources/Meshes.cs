using Godot;

namespace OpenTrenches.Core.Scripting.Libraries;

public static class Meshes
{
    public static MeshLibrary TerrainLibrary = (MeshLibrary)ResourceLoader.Singleton.Load("Mesh/Libraries/TerrainLibrary.tres");

    public static Mesh Dirt = (Mesh)ResourceLoader.Singleton.Load("Mesh/Dirt.res");
}