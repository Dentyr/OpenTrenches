using System;
using Godot;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Resources;
using OpenTrenches.Core.Scripting;
using OpenTrenches.Core.Scripting.Graphics;
using OpenTrenches.Core.Scripting.Player;

namespace OpenTrenches.Core.Scene.World;

public partial class CharacterRenderer : Area2D
{
    private IClientState _clientState { get; }
    private Character _character { get; }
    /// <summary>
    /// Sets local position to match <see cref="Character"/>'s position
    /// </summary>
    private void SyncPosition()
    {
        Position = _character.Position * CommonDefines.CellSize;
    }

    public bool OnPlayerTeam => _clientState.PlayerCharacter?.Team == _character.Team;
    public bool PlayerCharacter => _clientState.PlayerCharacter == _character;
    
    //* GD
    private CharacterFloat _floatLabel;
    private Sprite2D _sprite;


    public CharacterRenderer(IClientState ClientState, Character Character)
    {
        _clientState = ClientState;
        _character = Character;
        SyncPosition();

        _floatLabel = new(Character);
        AddChild(_floatLabel);

        _sprite = new()
        {
            Texture = TextureLibrary2D.Character.DefaultCharacter,
            Modulate = TeamModulate.GetColor(Character.Team == ClientState.PlayerCharacter?.Team),
        };
        _sprite.Scale = new Vector2(24f, 24f) / _sprite.Texture.GetSize();
        AddChild(_sprite);
    }

    public override void _Process(double delta)
    {        
        SyncPosition();
        _character.Process((float)delta);
    }

}
