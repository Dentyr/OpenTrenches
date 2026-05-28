using Godot;
using OpenTrenches.Common.Combat;
using OpenTrenches.Core.Scripting.Player;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class EquipmentUpgradeControl : Control
{
    public event Action<FirearmEnum>? EquipmentSelectedEvent;

    private const float SlideSeconds = 0.16f;

    private VBoxContainer _upgrades = null!;
    private Button _button = null!;
    private IReadOnlyPlayerState? _playerState;

    private readonly Dictionary<FirearmEnum, Button> _upgradeButtons = new();
    private readonly Dictionary<FirearmEnum, bool> _availability = new();
    private bool _hasCheckedAvailability;
    private bool _open = true;

    private float _openOffsetLeft;
    private float _openOffsetRight;
    private bool _sliding;
    private float _slideElapsed;
    private float _slideFromLeft;
    private float _slideFromRight;
    private float _slideTargetLeft;
    private float _slideTargetRight;

    public override void _Ready()
    {
        _upgrades = GetNode<VBoxContainer>("Upgrades");
        _button = GetNode<Button>("Button");

        _openOffsetLeft = OffsetLeft;
        _openOffsetRight = OffsetRight;

        _button.Disabled = true;
        _button.Pressed += ToggleUpgrades;
        SetOpen(false, immediate: true);
    }

    public void ShowUpgrades(PlayerState playerState)
        => ShowUpgrades((IReadOnlyPlayerState)playerState);

    public void ShowUpgrades(IReadOnlyPlayerState playerState)
    {
        _playerState = playerState;
        _hasCheckedAvailability = false;

        BuildUpgradeButtons();
        RefreshUpgradeAvailability();
    }

    public override void _Process(double delta)
    {
        ProcessSlide((float)delta);

        if (_playerState is null || _upgradeButtons.Count == 0) return;
        RefreshUpgradeAvailability();
    }

    private void BuildUpgradeButtons()
    {
        foreach (Node child in _upgrades.GetChildren()) child.QueueFree();

        _upgradeButtons.Clear();
        _availability.Clear();

        foreach (FirearmType upgrade in EquipmentTypes.All.Values.OrderBy(equipment => equipment.LogisticsCost))
        {
            FirearmEnum upgradeId = upgrade.Id;
            Button upgradeButton = new()
            {
                Text = $"{upgrade.Name} ({upgrade.LogisticsCost})",
                TooltipText = $"{upgrade.Description}\nCost: {upgrade.LogisticsCost} logistics",
                ClipText = true,
                SizeFlagsHorizontal = SizeFlags.ExpandFill,
            };

            upgradeButton.Pressed += () => TrySelectUpgrade(upgradeId);
            _upgrades.AddChild(upgradeButton);

            _upgradeButtons[upgradeId] = upgradeButton;
            _availability[upgradeId] = false;
        }
    }

    private void RefreshUpgradeAvailability()
    {
        bool hasAvailableUpgrade = false;
        bool newlyAvailableUpgrade = false;

        foreach ((FirearmEnum upgrade, Button button) in _upgradeButtons)
        {
            bool isAvailable = CanPurchase(upgrade);
            bool wasAvailable = _availability.TryGetValue(upgrade, out bool previous) && previous;

            button.Disabled = !isAvailable;
            _availability[upgrade] = isAvailable;

            hasAvailableUpgrade |= isAvailable;
            newlyAvailableUpgrade |= isAvailable && !wasAvailable;
        }

        _button.Disabled = !hasAvailableUpgrade;

        if (!hasAvailableUpgrade) SetOpen(false);
        else if (!_hasCheckedAvailability || newlyAvailableUpgrade) SetOpen(true);

        _hasCheckedAvailability = true;
        UpdateToggleText();
    }

    private bool CanPurchase(FirearmEnum upgrade)
    {
        if (_playerState is null) return false;
        return _playerState.Logistics >= EquipmentTypes.Get(upgrade).LogisticsCost;
    }

    private void ToggleUpgrades()
    {
        if (_button.Disabled) return;

        SetOpen(!_open);
    }

    private void UpdateToggleText()
    {
        if (_button is null || _upgrades is null) return;
        _button.Text = _open ? ">" : "<";
    }

    private void SetOpen(bool open, bool immediate = false)
    {
        if (_upgrades is null || _button is null) return;
        if (_open == open && !immediate)
        {
            UpdateToggleText();
            return;
        }

        _open = open;

        (float targetLeft, float targetRight) = GetTargetOffsets(open);

        if (open) _upgrades.Visible = true;

        if (immediate || SlideSeconds <= 0)
        {
            _sliding = false;
            OffsetLeft = targetLeft;
            OffsetRight = targetRight;
            _upgrades.Visible = open;
            UpdateToggleText();
            return;
        }

        _slideElapsed = 0;
        _slideFromLeft = OffsetLeft;
        _slideFromRight = OffsetRight;
        _slideTargetLeft = targetLeft;
        _slideTargetRight = targetRight;
        _sliding = true;
        UpdateToggleText();
    }

    private (float Left, float Right) GetTargetOffsets(bool open)
    {
        if (open) return (_openOffsetLeft, _openOffsetRight);

        float width = _openOffsetRight - _openOffsetLeft;
        return (_openOffsetRight, _openOffsetRight + width);
    }

    private void ProcessSlide(float delta)
    {
        if (!_sliding) return;

        _slideElapsed += delta;
        float progress = Mathf.Clamp(_slideElapsed / SlideSeconds, 0, 1);
        float eased = 1 - ((1 - progress) * (1 - progress) * (1 - progress));

        OffsetLeft = Mathf.Lerp(_slideFromLeft, _slideTargetLeft, eased);
        OffsetRight = Mathf.Lerp(_slideFromRight, _slideTargetRight, eased);

        if (progress < 1) return;

        _sliding = false;
        OffsetLeft = _slideTargetLeft;
        OffsetRight = _slideTargetRight;
        _upgrades.Visible = _open;
    }

    private void TrySelectUpgrade(FirearmEnum upgrade)
    {
        if (!CanPurchase(upgrade)) return;
        EquipmentSelectedEvent?.Invoke(upgrade);
    }
}
