using Godot;

namespace OpenTrenches.Core.Scene.Combat;

/// <summary>
/// Hp bar for characters
/// </summary>
public abstract partial class AbstractHpBar : ProgressBar
{    
    public AbstractHpBar()
    {
        MinValue = 0;
        MaxValue = 1;
        Step = 0.01;
        ShowPercentage = false;

        AddThemeStyleboxOverride("background", CreateStyleBox(new Color(0.18f, 0.18f, 0.18f, 0.9f), true));
        AddThemeStyleboxOverride("fill", CreateStyleBox(new Color(0.75f, 0.02f, 0.03f)));
    }

    private static StyleBoxFlat CreateStyleBox(Color color, bool bordered = false)
    {
        var styleBox = new StyleBoxFlat()
        {
            BgColor = color,
        };

        if (bordered)
        {
            styleBox.BorderColor = Colors.Black;
            styleBox.SetBorderWidthAll(1);
        }

        return styleBox;
    }
}
