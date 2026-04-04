using System;
using Godot;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Resources;
using OpenTrenches.Core.Scripting;
using OpenTrenches.Core.Scripting.Player;

namespace OpenTrenches.Core.Scene.World;

public partial class CharacterRenderer : Area2D
{
    private IClientState _clientState { get; }
    private Character _character { get; }

    public bool OnPlayerTeam => _clientState.PlayerCharacter?.Team == _character.Team;
    public bool PlayerCharacter => _clientState.PlayerCharacter == _character;
    
    //* GD
    private CharacterFloat _floatLabel;
    private Sprite2D _sprite;


    public CharacterRenderer(IClientState ClientState, Character Character)
    {
        _clientState = ClientState;
        _character = Character;
        Position = new(Character.Position.X, Character.Position.Z);

        _floatLabel = new(Character);
        AddChild(_floatLabel);

        _sprite = new();
        AddChild(_sprite);
        _sprite.Texture = TextureLibrary2D.Character.DefaultCharacter;

        AddChild(new MeshInstance2D()
        {
            Mesh = Meshes.Dirt,
            Material = OnPlayerTeam ? Materials.WhiteDebug : Materials.RedDebug,
        });
        AddChild(new CollisionShape2D()
        {
            Shape = new RectangleShape2D()
            {
                Size = new(1, 1),
            }
        });
    }

    public override void _Process(double delta)
    {        
        Position = new Vector2(_character.Position.X, _character.Position.Z) * CommonDefines.CellSize;
        _character.Process((float)delta);
    }

}
