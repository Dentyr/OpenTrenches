using System;
using Godot;
using OpenTrenches.Common.Combat;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Resources;
using OpenTrenches.Common.Scene;
using OpenTrenches.Core.Scene.Combat;
using OpenTrenches.Core.Scripting;
using OpenTrenches.Core.Scripting.Graphics;
using OpenTrenches.Core.Scripting.Player;

namespace OpenTrenches.Core.Scene.World;

public partial class CharacterRenderer : Area2D
{
    private readonly IClientState _clientState;
    public Character Character { get; }
    /// <summary>
    /// Sets local position to match <see cref="Scripting.Player.Character"/>'s position
    /// </summary>
    private void SyncPosition()
    {
        Position = Character.Position * CommonDefines.CellSize;
    }
    private void SyncWeaponRotation()
    {
        if (_weaponSprite is not null) 
            _weaponSprite.Rotation = Character.Rotation;
    }

    public bool OnPlayerTeam => _clientState.PlayerCharacter?.Team == Character.Team;
    public bool PlayerCharacter => _clientState.PlayerCharacter == Character;
    
    //* GD
    private readonly CharacterHpBar _hpBar;
    
    private readonly Sprite2D _bodySprite;
    private readonly Sprite2D _weaponSprite;


    private readonly CollisionShape2D _hitbox;


    public CharacterRenderer(IClientState clientState, Character character)
    {
        _clientState = clientState;
        Character = character;
        character.OnPrimaryChangedEvent += UpdateWeaponTexture;
        SyncPosition();

        _hpBar = new(character);
        AddChild(_hpBar);

        _bodySprite = new()
        {
            Texture = TextureLibrary2D.Character.DefaultCharacter,
            Modulate = TeamModulate.GetColor(character.Team == clientState.PlayerCharacter?.Team),
        };
        _bodySprite.Scale = new Vector2(24f, 24f) / _bodySprite.Texture.GetSize();
        AddChild(_bodySprite);

        _weaponSprite = new()
        {
            Texture = TextureLibrary2D.Character.Rifle,
            Modulate = TeamModulate.GetColor(character.Team == clientState.PlayerCharacter?.Team),
        };
        _weaponSprite.Scale = new Vector2(24f, 24f) / _weaponSprite.Texture.GetSize();
        AddChild(_weaponSprite);

        _hitbox = new()
        {
            Shape = new CircleShape2D()
            {
                Radius = CommonDefines.CharacterRadius,
            },
        };
        AddChild(_hitbox);

        CollisionMask = PhysicsDefines.Map.NilLayer;
        SetCollisionEnabled(character.IsActive);

        character.ActivatedEvent += ActivateCollision;
        character.InactivatedEvent += DeactivateCollision;
    }

    private void UpdateWeaponTexture(FirearmEnum firearm)
    {
        Texture2D texture = firearm switch
        {
            FirearmEnum.Shotgun => TextureLibrary2D.Character.Shotgun,
            FirearmEnum.MachineGun => TextureLibrary2D.Character.Machingun,
            _ => TextureLibrary2D.Character.Rifle,
        };
        _weaponSprite.Texture = texture;
        _weaponSprite.Scale = new Vector2(24f, 24f) / _weaponSprite.Texture.GetSize();
    }

    public override void _Process(double delta)
    {        
        SyncPosition();
        SyncWeaponRotation();
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
