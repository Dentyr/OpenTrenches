using System;
using Godot;
using OpenTrenches.Common.Resources;
using OpenTrenches.Core.Scripting;
using OpenTrenches.Core.Scripting.Player;

namespace OpenTrenches.Core.Scene.World;

public partial class CharacterRenderer : Node3D
{
    private IClientState _clientState { get; }
    private Character _character { get; }
    
    public bool OnPlayerTeam => _clientState.PlayerCharacter?.Team == _character.Team;
    public bool PlayerCharacter => _clientState.PlayerCharacter == _character;

    public CharacterRenderer(IClientState ClientState, Character Character)
    {
        _clientState = ClientState;
        _character = Character;
        Position = Character.Position;
        AddChild(new MeshInstance3D()
        {
            Mesh = Meshes.Dirt,
            MaterialOverride = OnPlayerTeam ? Materials.WhiteDebug : Materials.RedDebug,
        });
        AddChild(new CollisionShape3D()
        {
            Shape = new BoxShape3D()
            {
                Size = new(1, 2, 1),
            }
        });
    }

    public override void _Process(double delta)
    {        
        Position = _character.Position;
        _character.Process((float)delta);
    }

}
