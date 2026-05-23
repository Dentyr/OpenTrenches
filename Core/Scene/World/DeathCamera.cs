using Godot;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Core.Scripting.World;

namespace OpenTrenches.Core.Scene.World;

/// <summary>
/// Camera that looks at valid targets for a dead player, such as respawn bases
/// </summary>
public partial class DeathCamera : Camera2D
{
    private ClientStructure? _focusCamp;

    public DeathCamera()
    {
        
    }

    public void Follow(ClientStructure focusCamp)
    {
        _focusCamp = focusCamp;
    }

    public override void _Process(double delta)
    {
        if (IsVisibleInTree() && _focusCamp is not null)
            Position = _focusCamp.Position.CellToPosition() * CommonDefines.CellSize;
    }
}