using Godot;
using OpenTrenches.Common.Collections;
using OpenTrenches.Common.Contracts.DTO;
using OpenTrenches.Common.Contracts.DTO.PlayerCommands;
using OpenTrenches.Core.Scene.GUI;
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
    

    private PolledQueue<AbstractCommandDTO> _queuedCommands { get; } = new();

    public override void _Ready()
    {
        _abilityContainer = GetNode<Container>("Abilities");

        _healthBar = GetNode<ProgressBar>("HpBar");
        _healthBar.MaxValue = 1;
        _healthBar.Step = 0.01;
        _healthLabel = _healthBar.GetNode<Label>("Label");
    }

    public void SetPlayer(Character character)
    {
        _character = character;
        foreach(Node? child in _abilityContainer.GetChildren()) child?.QueueFree();
        for(int abilityIdx = 0; abilityIdx < character.GetAbilities().Count; abilityIdx ++)
        {
            var abilityNode = new AbilityButton(character, abilityIdx);
            abilityNode.AbilitySelected += NotifyAbilityCommand; 
            _abilityContainer.AddChild(abilityNode);
        }
    }

    private void NotifyAbilityCommand(int idx) => _queuedCommands.Enqueue(new UseAbilityCommandRequest(0));


    public override void _Process(double delta)
    {
        if (_character is not null)
        {
            _healthLabel.Text = _character.Health + "/" + _character.MaxHealth;
            _healthBar.Value = _character.Health / _character.MaxHealth;
        }
    }

    public IEnumerable<AbstractCommandDTO> PollCommands() => _queuedCommands.PollItems();
}


