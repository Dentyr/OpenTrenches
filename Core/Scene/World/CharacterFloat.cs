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
        // var cam := $Camera3D
        var screenPos = GetViewport().GetCamera3D().UnprojectPosition(Character.Position);
        this.Text = Character.Health.ToString();
        Position = screenPos;
    }
}