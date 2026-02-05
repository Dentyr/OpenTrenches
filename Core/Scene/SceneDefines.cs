using Godot;
namespace OpenTrenches.Core.Scene;

public static class SceneDefines
{
    public static Environment IlluminatedEnvironment { get; } = new Environment()
    {
        BackgroundMode = Environment.BGMode.Color,
        AmbientLightColor = Colors.White,
        AmbientLightEnergy = 0.1f,
        AmbientLightSource = Environment.AmbientSource.Color,
    };
}