using Godot;

namespace OpenTrenches.Scene.Resources;

public static class Materials
{
    public static readonly StandardMaterial3D PinkDebug = new()
    {
        AlbedoColor = Colors.Pink,
    };
}