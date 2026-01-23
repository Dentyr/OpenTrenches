using Godot;

namespace OpenTrenches.Core.Scene.Resources;

public static class Materials
{
    public static readonly StandardMaterial3D PinkDebug = new()
    {
        AlbedoColor = Colors.Pink,
    };
}