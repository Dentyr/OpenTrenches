using Godot;
using OpenTrenches.Common.Collections;
using OpenTrenches.Common.Combat;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Contracts.DTO;
using OpenTrenches.Common.Contracts.DTO.PlayerCommands;
using OpenTrenches.Core.Scene.GUI;
using OpenTrenches.Core.Scripting;
using OpenTrenches.Core.Scripting.Player;
using System;
using System.Collections.Generic;

namespace OpenTrenches.Core.Scene.Gui;

public partial class CharacterControlUi : Control
{
    private Character? _character;

    private Container _abilityContainer = null!;

    public event Action<int>? AbilitySelected;

    private ProgressBar _healthBar = null!;
    private Label _healthLabel = null!;

    private IconDisplay _logisticsDisplay = null!;

    //* equipment & upgrades

    private FirearmSlotDisplay _primarySlotDisplay = null!;

    private EquipmentUpgradePanel _equipmentUpgradePanel = null!;
    

    //* user commands

    private PolledQueue<AbstractCommandDTO> _queuedCommands { get; } = new();

    public override void _Ready()
    {
        _abilityContainer = GetNode<Container>("Abilities");

        _healthBar = GetNode<ProgressBar>("HpBar");
        _healthBar.MaxValue = 1;
        _healthBar.Step = 0.01;
        _healthLabel = _healthBar.GetNode<Label>("Label");

        //*

        _primarySlotDisplay = GetNode<FirearmSlotDisplay>("PrimarySlot");
        _equipmentUpgradePanel = GetNode<EquipmentUpgradePanel>("EquipmentUpgradePanel");

        //*

        _logisticsDisplay = GetNode<IconDisplay>("LogisticsDisplay");


        //* events
        _equipmentUpgradePanel.EquipmentSelectedEvent += RequestUpgrade;
    }

    public void SetPlayer(LocalPlayerView player)
    {
        _primarySlotDisplay.SetState(player.PlayerState.PrimarySlotState);

        _character = player.Character;


        foreach(Node? child in _abilityContainer.GetChildren()) child?.QueueFree();
        for(int abilityIdx = 0; abilityIdx < _character.GetAbilities().Count; abilityIdx ++)
        {
            var abilityNode = new AbilityButton(_character, abilityIdx);
            abilityNode.AbilitySelected += NotifyAbilityCommand; 
            _abilityContainer.AddChild(abilityNode);
        }

        _character.OnPrimaryChangedEvent += _primarySlotDisplay.SetEquipment;
        _primarySlotDisplay.SetEquipment(_character.Primary);
    }
    public void SetLogistics(int value)
    {
        _logisticsDisplay.Text = value.ToString();
    }

    public void NotifyPlayerFire()
    {
        _primarySlotDisplay.StartFireTimer();
    }

    public void NotifyPlayerReload()
    {
        _primarySlotDisplay.StartReloadTimer();
    }

    private void NotifyAbilityCommand(int idx) => _queuedCommands.Enqueue(new UseAbilityCommandRequest(0));
    private void RequestUpgrade(FirearmEnum equipment) => _queuedCommands.Enqueue(new PurchaseCommandRequest(equipment));


    public override void _Process(double delta)
    {
        if (_character is not null)
        {
            _healthLabel.Text = _character.Health + "/" + CommonDefines.MaxHealth;
            _healthBar.Value = _character.Health / CommonDefines.MaxHealth;
        }
    }

    public IEnumerable<AbstractCommandDTO> PollCommands() => _queuedCommands.PollItems();


}


