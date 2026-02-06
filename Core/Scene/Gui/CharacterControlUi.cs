using Godot;
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
    

    private List<AbstractCommandDTO> _queuedCommands { get; } = [];

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

    private void NotifyAbilityCommand(int idx) => _queuedCommands.Add(new UseAbilityCommandRequest(0));

    public IEnumerable<AbstractCommandDTO> PollCommands()
    {
        var temp = _queuedCommands.ToArray();
        _queuedCommands.Clear();
        return temp;
    }
}


