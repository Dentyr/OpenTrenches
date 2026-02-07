using Godot;
using OpenTrenches.Common.Resources;
using OpenTrenches.Core.Scripting.Ability;
using OpenTrenches.Core.Scripting.Player;
using System;

namespace OpenTrenches.Core.Scene.GUI;

[GlobalClass]
public partial class AbilityButton : Control
{
    private Character Character { get; }
    private int AbilityIndex { get; }

    private Button _button;

    private TextureProgressBar _timerProgress;

    private TextureProgressBar _durationProgress;

    public event Action<int>? AbilitySelected;

    public AbilityButton(Character Character, int AbilityIndex)
    {
        this.AbilityIndex = AbilityIndex;
        this.Character = Character;

        CustomMinimumSize = new(50, 50);

        //* Button

        _button = new()
        {
            Icon = AbilityTextureRecordLibrary.Textures[Character.GetAbility(AbilityIndex).Record].Thumbnail,
            ExpandIcon = true,
        };
        _button.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        AddChild(_button);
        
        _button.Pressed += HandlePressed;


        //* Progress Bar

        _timerProgress = InitTimerProgress();
        _timerProgress.TextureProgress = TextureLibrary2D.TransparentGray;

        _durationProgress = InitTimerProgress();
        _durationProgress.TextureProgress = TextureLibrary2D.Cyan;


    }
    private TextureProgressBar InitTimerProgress()
    {
        var progress = new TextureProgressBar()
        {
            Visible = false,
            FillMode = (int)TextureProgressBar.FillModeEnum.CounterClockwise,
            MaxValue = 1,
            Step = 0.01,
            MinValue = 0,
            NinePatchStretch = true,
        };
        progress.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        AddChild(progress);
        return progress;
    }

    public override void _Process(double delta)
    {
        var ability = Character.GetAbility(AbilityIndex);

        // Show cooldown time
        _timerProgress.Value = ability.TimeLeft / ability.Record.Cooldown;
        SetTimerVisibility(_timerProgress);

        // Show active duration
        _durationProgress.Value = ability.Duration / ability.Record.Duration;
        SetTimerVisibility(_durationProgress);

    }
    private void SetTimerVisibility(TextureProgressBar progressBar)
    {
        if (progressBar.Value > 0 && !progressBar.Visible) progressBar.Visible = true;
        else if (progressBar.Value <= 0 && progressBar.Visible) progressBar.Visible = false;
    }

    private void HandlePressed()
    {
        AbilitySelected?.Invoke(AbilityIndex);
    }
}
