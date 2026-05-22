using Godot;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Core.Scripting.Player;

namespace OpenTrenches.Core.Scene.Combat;


/// <summary>
/// Hp bar for characters
/// </summary>
public partial class CharacterHpBar : AbstractHpBar
{
    private static readonly Vector2 BarSize = new(24f, 4f);

    private readonly Character _character;
    
    public CharacterHpBar(Character character)
    {
        _character = character;

        CustomMinimumSize = BarSize;
        Size = BarSize;
        Position = new Vector2(-BarSize.X / 2f, CommonDefines.CharacterRadius + BarSize.Y + 4f);
    }

    public override void _Process(double delta)
    {
        Value = Mathf.Clamp(_character.Hp / CommonDefines.MaxHp, 0f, 1f);
    }
}
