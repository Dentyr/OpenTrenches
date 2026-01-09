using Godot;

namespace OpenTrenches.Scene.World;

public partial class WorldNode : Node3D
{
    public Character Character = new();

    public void DisablePhysics()
    {
    }
}