using Godot;
using OpenTrenches.Common.Contracts.Defines;
using System;

public partial class DeathScreen : Control
{
    private Button _respawnButton = null!;
    public event Action? OnRespawnClicked;

    private float TimeLeft = 0f;

    public override void _Ready()
    {
        _respawnButton = GetNode<Button>("Respawn");
        _respawnButton.Pressed += () => OnRespawnClicked?.Invoke();
    }

    public override void _Process(double delta)
    {
        TimeLeft -= (float)delta;
        if (TimeLeft > 0)
        {
            if (!_respawnButton.Disabled)
                _respawnButton.Disabled = true;

            _respawnButton.Text = $"{Mathf.CeilToInt(TimeLeft):#} seconds left";
        }
        else
        {
            if (_respawnButton.Disabled)
                _respawnButton.Disabled = false;
            
            _respawnButton.Text = $"respawn";
        }
    }

    /// <summary>
    /// Show and prompt the user to respawn
    /// </summary>
    public void Prompt()
    {
        TimeLeft = CommonDefines.SecondsForCharacterRespawn;
        Visible = true;

    }



}
