using Godot;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.World;
using OpenTrenches.Core.Scripting.World;

namespace OpenTrenches.Core.Scene.Combat;

/// <summary>
/// Hp bar for characters
/// </summary>
public partial class StructureHpBar : AbstractHpBar
{
    private static readonly Vector2 BarSize = new(86f, 4f);

    private readonly ClientStructure _structure;
    private readonly StructureType _type;
    
    public StructureHpBar(ClientStructure structure)
    {
        _structure = structure;
        _type = StructureTypes.All[structure.Enum];
        
        CustomMinimumSize = BarSize;
        Size = BarSize;
        Position = new Vector2(-BarSize.X / 2f, 30 + BarSize.Y + 4f);
    }

    public override void _Process(double delta)
    {
        Value = Mathf.Clamp(_structure.Hp / _type.Hp, 0f, 1f);
        if (Value == 1f)
        {
            if (!Visible) Visible = false;
        }
        else if (!Visible) Visible = true;
    }
}
