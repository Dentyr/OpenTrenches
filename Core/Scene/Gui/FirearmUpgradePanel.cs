using System;
using System.Collections.Generic;
using Godot;
using OpenTrenches.Common.Combat;

[GlobalClass]
[Tool]
public partial class FirearmUpgradePanel : EquipmentUpgradePanel<FirearmEnum>
{
    private readonly Dictionary<string, FirearmEnum> _upgradesById = new();

    public override void _Ready()
    {
        base._Ready();
        SetUpgrades(Enum.GetValues<FirearmEnum>());
    }
}
