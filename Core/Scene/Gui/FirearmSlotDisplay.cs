using System;
using Godot;
using OpenTrenches.Common.Combat;
using OpenTrenches.Common.Resources;
using OpenTrenches.Core.Scripting.Combat;
using OpenTrenches.Core.Scripting.Graphics;

[GlobalClass]
[Tool]
public partial class FirearmSlotDisplay : Control
{
    private FirearmSlot? _slot;

    private readonly Label _hpLabel;
    private readonly TextureRect _texture;
    private readonly TextureProgressBar _cooldownBar;
    private readonly TextureProgressBar _reloadBar;

    public FirearmSlotDisplay()
    {
        _texture = new TextureRect()
        {
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
        };
        _texture.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        AddChild(_texture);

        _cooldownBar = InitRadialProgressBar();
        _cooldownBar.TextureProgress = TextureLibrary2D.TransparentGray;

        _reloadBar = InitRadialProgressBar();
        _reloadBar.TextureProgress = TextureLibrary2D.Cyan;

        _hpLabel = new Label()
        {
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Bottom,
        };
        _hpLabel.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        AddChild(_hpLabel);
    }

    private TextureProgressBar InitRadialProgressBar()
    {
        var progressBar = new TextureProgressBar()
        {
            FillMode = (int)TextureProgressBar.FillModeEnum.CounterClockwise,
            MinValue = 0,
            MaxValue = 1,
            Step = 0.01,
            NinePatchStretch = true,
        };
        progressBar.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        AddChild(progressBar);
        return progressBar;
    }

    //* updating

    public void SetSlot(FirearmSlot? slot)
    {
        _slot = slot;
        UpdateTexture();
    }
    private void UpdateTexture()
    {
        if (_slot?.Equipment is AbstractEquipmentType equipmentType 
            && EquipmentTextureLibrary.Textures.TryGetValue(equipmentType.Id, out var texture)) 
        {
            _texture.Texture = texture;
        }
    }

    public override void _Process(double delta)
    {
        if (_slot is null)
        {
            _texture.Texture = null;
        }
        if (_slot is null || _slot.Equipment is null)
        {
            _cooldownBar.Value = 0;
            _reloadBar.Value = 0;
            _cooldownBar.Visible = false;
            _reloadBar.Visible = false;
            return;
        }


        var stats = _slot.Equipment.Stats;
        var maxFireCooldown = stats.RateOfFire > 0 ? 60f / stats.RateOfFire : 0f;
        var maxReloadCooldown = stats.ReloadSeconds;
        _hpLabel.Text = _slot.AmmoLoaded.ToString();

        _cooldownBar.Value = Mathf.Clamp(_slot.FireCooldown / maxFireCooldown, 0f, 1f);
        _reloadBar.Value = Mathf.Clamp(_slot.ReloadCooldown / maxReloadCooldown, 0f, 1f);

        SetTimerVisibility(_cooldownBar);
        SetTimerVisibility(_reloadBar);
    }

    private void SetTimerVisibility(TextureProgressBar progressBar)
    {
        if (progressBar.Value > 0 && !progressBar.Visible) progressBar.Visible = true;
        else if (progressBar.Value <= 0 && progressBar.Visible) progressBar.Visible = false;
    }
}
