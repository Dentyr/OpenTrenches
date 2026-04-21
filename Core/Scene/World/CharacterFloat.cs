using Godot;
using OpenTrenches.Core.Scripting.Player;

namespace OpenTrenches.Core.Scene.World;

public partial class CharacterFloat : Label
{
    private Character Character { get; }
    public CharacterFloat(Character Character)
    {
        this.Character = Character;
    }
    public override void _Process(double delta)
    {
        Text = Character.Hp.ToString();
    }
}