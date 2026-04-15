using Godot;
using OpenTrenches.Core.Scripting;
using OpenTrenches.Core.Scripting.Teams;
using System;

public partial class GameEndScreen : Control
{
    private Label _result = null!;
    private Button _returnButton = null!;
    
    public event Action? ReturnRequestEvent;

    public override void _Ready()
    {
        _result = GetNode<Label>("Label");
        _returnButton = GetNode<Button>("Return");

        _returnButton.Pressed += () => ReturnRequestEvent?.Invoke();
    }
    public void ShowEnd(ClientTeam victor, IClientState state)
    {
        if (victor.ID == state.PlayerCharacter?.Team)
            _result.Text = "Victory!";
        else 
            _result.Text = "Defeated by enemy " + victor.Faction.Name + "!";

        Visible = true;
    }
}
