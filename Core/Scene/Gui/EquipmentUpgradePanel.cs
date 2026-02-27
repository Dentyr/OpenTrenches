using System;
using System.Collections;
using System.Collections.Generic;
using Godot;
using OpenTrenches.Common.Combat;

/// <summary>
/// An upgrade panel for a <see cref="IReadOnlyEquipmentSlot"/>
/// </summary>
[GlobalClass]
[Tool]
public partial class EquipmentUpgradePanel : Control 
{
    public event Action<FirearmEnum>? EquipmentSelectedEvent;

    private Button _toggle;
    private VBoxContainer _menu;

    [Export]
    public bool Open
    {
        get => _menu.Visible;
        set => _menu.Visible = value;
    }

    public EquipmentUpgradePanel()
    {
        _toggle = new()
        {
            Text = "+",
            ClipText = true,
        };
        _toggle.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);

        _menu = new()
        {
            Visible = false,
            GrowVertical = GrowDirection.Begin,
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
        };
        _menu.SetAnchorsPreset(LayoutPreset.BottomWide);


        _toggle.Pressed += ToggleOpen;



        AddChild(_toggle);
        AddChild(_menu);
    }
    public override void _Ready()
    {
        //TODO debug set
        SetUpgrades(Enum.GetValues<FirearmEnum>());

    }

    private void ToggleOpen()
    {
        Open = !Open;
    }

    public override void _Process(double delta)
    {
        if (Open) PositionMenu();
    }

    private void PositionMenu()
    {
        // Magic numbers / simple behavior: menu appears above toggle, same width.
        const float gap = 6f;
        const float maxHeight = 260f;

        _menu.Position = new Vector2(_toggle.Position.X, _toggle.Position.Y - _menu.Size.Y - gap);
        _menu.Size = new Vector2(_toggle.Size.X, Mathf.Min(_menu.Size.Y, maxHeight));
    }

    public void SetUpgrades(IEnumerable<FirearmEnum> upgrades)
    {
        _menu.QueueFreeChildren();

        foreach(FirearmEnum upgrade in upgrades)
        {
            Button button = new()
            {
                Text = upgrade.ToString(),
                ClipText = true,
            };
            button.Pressed += () => EquipmentSelectedEvent?.Invoke(upgrade);
            _menu.AddChild(button);
        }
    }
}
