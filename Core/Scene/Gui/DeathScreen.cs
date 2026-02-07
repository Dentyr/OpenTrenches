using Godot;
using System;

public partial class DeathScreen : Control
{
    private Button _respawnButton = null!;
    public event Action? OnRespawnClicked;

    public override void _Ready()
    {
        _respawnButton = GetNode<Button>("Respawn");
        _respawnButton.Pressed += () => OnRespawnClicked?.Invoke();
    }



}
