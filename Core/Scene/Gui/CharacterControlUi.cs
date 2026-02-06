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
    private Container _abilityContainer { get; set; } = null!;

    public event Action<int>? AbilitySelected;
    

    private PolledQueue<AbstractCommandDTO> _queuedCommands { get; } = new();

    public override void _Ready()
    {
        _abilityContainer = GetNode<Container>("Abilities");
    }

    public void SetPlayer(Character character)
    {
        foreach(Node? child in _abilityContainer.GetChildren()) child?.QueueFree();
        for(int abilityIdx = 0; abilityIdx < character.GetAbilities().Count; abilityIdx ++)
        {
            var abilityNode = new AbilityButton(character, abilityIdx);
            abilityNode.AbilitySelected += NotifyAbilityCommand; 
            _abilityContainer.AddChild(abilityNode);
        }
    }

    private void NotifyAbilityCommand(int idx) => _queuedCommands.Enqueue(new UseAbilityCommandRequest(0));

    public IEnumerable<AbstractCommandDTO> PollCommands() => _queuedCommands.PollItems();
}


