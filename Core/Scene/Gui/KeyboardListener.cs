using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MessagePack;
using OpenTrenches.Common.Contracts.DTO;
using OpenTrenches.Core.Scripting;
using OpenTrenches.Core.Scripting.Player;

public partial class KeyboardListener : Node
{
    public bool W { get; set; }
    public bool A { get; set; }
    public bool S { get; set; }
    public bool D { get; set; }
    public bool LMB { get; set; } //left mouse button
    
    public Vector2 MPos { get; set; } //mouse position

    private Character? Character { get; set; }
    public void SetPlayer(Character character) => Character = character;


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
    }
    public override void _Process(double delta)
    {
        Camera3D camera = GetViewport().GetCamera3D();
        if (camera is null) return;
        Vector2 mousePos = GetViewport().GetMousePosition();

        // Project point in screen to world origin and direction
        Vector3 origin = camera.ProjectRayOrigin(mousePos);
        Vector3 direction = camera.ProjectRayNormal(mousePos);

        // From the ray projection, find the position level to the character.
        Vector3 intersect = FindIntersect(origin, direction, Character?.Position.Y ?? 0);
        this.MPos = new(intersect.X, intersect.Z);
    }
    /// <summary>
    /// Returns where the line from <paramref name="origin"/> to <paramref name="direction"/> intersects y=<paramref name="targetY"/>, or origin if close to a parallel line.
    /// </summary>
    private Vector3 FindIntersect(Vector3 origin, Vector3 direction, float targetY)
    {
        float dy = direction.Y;
        // float dy = direction.Y - origin.Y;
        if (Math.Abs(dy) < 1e-3) return origin;

        float t = (targetY - origin.Y) / dy;
        Vector3 intersection = origin + t * direction;
        return intersection;
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

    public InputStatusDTO GetStatus()
    {
        return new InputStatusDTO()
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

