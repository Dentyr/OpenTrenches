using System;
using Godot;
using OpenTrenches.Scripting.Player;

namespace OpenTrenches.Scene.World;

public partial class WorldNode : Node3D
{
    public Character? Character;
    public void AddCharacter(Character character)
    {
        Character = character;
        CharacterNode3D node = new(character);
        AddChild(node);
        Console.WriteLine("Adding character");
    }
    public void DisablePhysics()
    {
    }
}