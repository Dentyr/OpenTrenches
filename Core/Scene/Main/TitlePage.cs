using Godot;
using System;

public partial class TitlePage : Control
{
    public event Action? NavigateServersEvent;

    public override void _Ready()
    {
        Button startButton = GetNode<Button>("Button");
        startButton.Pressed += () => NavigateServersEvent?.Invoke();
    }
}
