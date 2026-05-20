using System;
using Godot;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Resources;
using OpenTrenches.Common.Scene;
using OpenTrenches.Core.Scripting;
using OpenTrenches.Core.Scripting.Graphics;
using OpenTrenches.Core.Scripting.Player;

namespace OpenTrenches.Core.Scene.World;

public partial class CharacterRenderer : Area2D
{
    private IClientState _clientState { get; }
    public Character Character { get; }
    /// <summary>
    /// Sets local position to match <see cref="Scripting.Player.Character"/>'s position
    /// </summary>
    private void SyncPosition()
    {
        Position = Character.Position * CommonDefines.CellSize;
    }

    public bool OnPlayerTeam => _clientState.PlayerCharacter?.Team == Character.Team;
    public bool PlayerCharacter => _clientState.PlayerCharacter == Character;
    
    //* GD
    private CharacterFloat _floatLabel;
    private Sprite2D _sprite;
    private CollisionShape2D _hitbox;


    public CharacterRenderer(IClientState ClientState, Character Character)
    {
        _clientState = ClientState;
        this.Character = Character;
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

        _hitbox = new()
        {
            Shape = new CircleShape2D()
            {
                Radius = CommonDefines.CharacterRadius,
            },
        };
        AddChild(_hitbox);

        CollisionMask = PhysicsDefines.Map.NilLayer;
        SetCollisionEnabled(Character.IsActive);

        Character.ActivatedEvent += ActivateCollision;
        Character.InactivatedEvent += DeactivateCollision;
    }

    public override void _Process(double delta)
    {        
        SyncPosition();
        Character.Process((float)delta);
    }

    private void ActivateCollision() => SetCollisionEnabled(true);
    private void DeactivateCollision() => SetCollisionEnabled(false);

    private void SetCollisionEnabled(bool enabled)
    {
        CollisionLayer = enabled ? PhysicsDefines.Map.CharacterLayer : PhysicsDefines.Map.NilLayer;
        _hitbox.Disabled = !enabled;
    }

}
