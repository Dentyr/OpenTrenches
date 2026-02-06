using Godot;

namespace OpenTrenches.Common.Resources;

public static class Materials
{
    public static readonly StandardMaterial3D PinkDebug = new()
    {
        AlbedoColor = Colors.Pink,
    };
    public static readonly StandardMaterial3D WhiteDebug = new()
    {
        AlbedoColor = Colors.White,
    };
}