using System.Collections.Generic;
using System.Linq;
using Godot;
using MessagePack;

public partial class KeyboardListener : Node
{
    public bool W { get; set; }
    public bool A { get; set; }
    public bool S { get; set; }
    public bool D { get; set; }
    public bool LMB { get; set; } //left mouse button
    
    public Vector2 MPos { get; set; } //mouse position



    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent)
        {
            switch (mouseEvent.ButtonIndex)
            {
                case MouseButton.Left:
                    LMB = mouseEvent.Pressed;
                    break;
                default:
                    break;
            }
        }
        else if (@event is InputEventMouseMotion motionEvent)
        {
            MPos = motionEvent.Position;
        }
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event is not InputEventKey keyEvent) return;

        switch(keyEvent.Keycode)
        {
            case Key.W:
                W = keyEvent.Pressed;
                break;
            case Key.A:
                A = keyEvent.Pressed;
                break;
            case Key.S:
                S = keyEvent.Pressed;
                break;
            case Key.D:
                D = keyEvent.Pressed;
                break;
            default:
                break;
        }
    }

    public InputStatus GetStatus()
    {
        return new InputStatus()
        {
            Keys = [.. GetKeyList()],
            MousePos = MPos,
        };
    }

    private IEnumerable<UserKey> GetKeyList()
    {
        if (W) yield return UserKey.W;
        if (A) yield return UserKey.A;
        if (S) yield return UserKey.S;
        if (D) yield return UserKey.D;
        if (LMB) yield return UserKey.LMB;
    }
}

[MessagePackObject]
public class InputStatus
{
    [Key(0)]
    public required UserKey[] Keys;
    [Key(1)]
    public required Vector2 MousePos;
}

public enum UserKey : byte
{
    W,
    A,
    S,
    D,
    LMB,
    RMB,
    R,
}