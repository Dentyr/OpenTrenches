
// namespace 
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using OpenTrenches.Common.Ability;
using OpenTrenches.Common.Combat;
using OpenTrenches.Common.Contracts;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Contracts.DTO.PlayerCommands;
using OpenTrenches.Common.Contracts.DTO.UpdateModel;
using OpenTrenches.Common.World;
using OpenTrenches.Server.Scripting.Ability;
using OpenTrenches.Server.Scripting.Adapter;
using OpenTrenches.Server.Scripting.Combat;
using OpenTrenches.Server.Scripting.Teams;
namespace OpenTrenches.Server.Scripting.Player;

public class Character : IIdObject
{
    //* Identification
    private IServerState ServerState { get; }
    public ushort ID { get; }
    public Team Team { get; }

    //* State in World
    
    public Vector3 Position { get; set; } = new (0, 10, 0);

    private readonly UpdateableProperty<Vector3> _direction = new(Vector3.Zero);
    /// <summary>
    /// The location this character is looking towards
    /// </summary>
    /// <value></value>
    public Vector3 Direction
    {
        get => _direction.Value;
        set => _direction.Value = value;
    }

    /// <summary>
    /// Cardinal velocity
    /// </summary>
    public Vector3 MovementVelocity { get; set; } = Vector3.Zero;

    private UpdateableProperty<CharacterState> _state = new(CharacterState.Idle); 
    public CharacterState State
    { 
        get => _state.Value;
        set => _state.Value = value;
    }

    //* Combat status
    private readonly UpdateableProperty<float> _health = new();
    public float Health 
    { 
        get => _health;
        private set => _health.Value = value;
    }


    private readonly UpdateableProperty<int> _logistics = new(100); //TODO debug logi
    public int Logistics
    {
        get => _logistics.Value;
        set => _logistics.Value = value;
    }
    private float _logisticsProgress = 0;

    private FirearmSlot _primarySlot = new(EquipmentTypes.Rifle);
    public IReadOnlyFirearmSlot PrimarySlot => _primarySlot;
    public void SwitchPrimary(FirearmType firearm)
    {
        _primarySlot.Equipment = firearm;
    }

    /// <summary>
    /// Called when the character is no longer active
    /// </summary>
    public event Action? DiedEvent;
    /// <summary>
    /// Called when the character becomes active
    /// </summary>
    public event Action? RespawnEvent;


    public event Action<Character, Vector3>? FireEvent;
    public event Action<Character>? ReloadEvent;

    
    //TODO Update stats to be more modular

    private float _baseDefense { get; set; } = 0;
    private float GetDefense()
    {
        return _baseDefense + Abilities.Sum(x => x.Active ? x.Record.DefenseMod : 0);
    }

    private CharacterAbility[] Abilities { get; } = [new(AbilityRecords.StimulantAbility)];
    public event Action<int>? ActivatedAbilityEvent;






    //* build target

    public TileType BuildTarget { get; private set; }
    public Vector2I BuildCell { get; private set; }


    public Character(IServerState ServerState, ushort ID, Team Team)
    {
        this.ServerState = ServerState;
        this.ID = ID;
        this.Team = Team;

        Respawn();
    }


    /// <summary>
    /// Called when the adapter simulates time passing
    /// </summary>
    /// <param name="delta"></param>
    /// <param name="adapter"></param>
    public void AdapterSimulate(float delta, ICharacterAdapter adapter)
    {
        //* logistics
        _logisticsProgress += delta / 3;
        if (_logisticsProgress >= 1)
        {
            _logisticsProgress --;
            Logistics ++;
        }

        // Cooldowns
        foreach (var abiltiy in this.Abilities) abiltiy.ProgressTimer(delta);
        _primarySlot.Cooldown(delta);
        
        if (State == CharacterState.Shooting) TryFire(adapter);


        //* building

        if (State == CharacterState.Building)
        {
            if (GetCell().DistanceTo(BuildCell) > CommonDefines.CellSize) 
            {
                CancelTask();
                return;
            }

            // only proceed if position is valid
            if (ServerState.Chunks.TryGetTile(BuildCell, out Tile? tile))
            {
                if (tile is null)
                {   //if no tile exists at position, make one with a build status.
                    ServerState.Chunks.StartBuild(BuildCell, BuildTarget, delta);
                }
                else if (tile.Type == BuildTarget)
                    CancelTask(); // already constructed, no further action
                else if (tile.Building is BuildStatus status && status.BuildTarget == BuildTarget) 
                {   // If build status is not null, make progress on building status if the target is the same
                    if (ServerState.Chunks.ProgressBuild(BuildCell, delta)) CancelTask();
                }
                else if (tile.Building is null)
                {   // if a tile exists, isn't the build target, and can be built, 
                    ServerState.Chunks.StartBuild(BuildCell, BuildTarget);
                }
                else CancelTask();
            }

            else CancelTask();
        }
    }
    private void TryFire(ICharacterAdapter adapter)
    {
        if (Position != Direction 
            && _primarySlot.Equipment is FirearmType firearm
            && _primarySlot.CanFire()
        ) {
            // human recoil spread
            var aimTarget = Position + (
                (Direction - Position)
                    .HSpread(_primarySlot.Recoil)
                    .Normalized() 
                * firearm.Stats.ProjectileDistance
            );
            for (int i = 0; i < firearm.Stats.ProjectilesPerShot; i ++)
            {
                
                // machien spread
                var kineticTarget = aimTarget;
                // var kineticTarget = aimTarget.BoxSpread(firearm.Stats.SpreadMOA);
                // var kineticTarget = Position + (
                //     (Direction - Position)
                //         .BoxSpread(firearm.Stats.SpreadMOA)
                //         .Normalized() 
                //     * firearm.Stats.ProjectileDistance
                // );
                
                FireHitResult result = adapter.AdaptFire(kineticTarget);
                if (result is FireHitResult.Hit hit && hit.Character.Team != Team) hit.Character.ApplyDamage(_primarySlot.Equipment.Stats.DamagePerProjectile);
                
                FireEvent?.Invoke(this, result.Position);
            }
            // Apply recoil
            _primarySlot.UseBullet();
            _primarySlot.ApplyCooldownAndRecoil();
        }
    }

    private void CancelTask()
    {
        State = CharacterState.Idle;
    }

    public void TrySwitch(CharacterState newState)
    {
        switch (newState)
        {
            case CharacterState.Idle:
                break;
            case CharacterState.Aiming:
                break;
            case CharacterState.Shooting:
                if (State == CharacterState.Reloading) return;
                break;
            case CharacterState.Reloading:
                break;
            case CharacterState.Building:
                break;
        }
        State = newState;
    }

    //* combat

    /// <summary>
    /// Sets character back to full health and at team spawnpoint
    /// </summary>
    public void Respawn()
    {
        Health = CommonDefines.MaxHealth; 
        Position = Team.SpawnPoint;
        RespawnEvent?.Invoke();
    }

    /// <summary>
    /// Attempt by the character to respawn.
    /// </summary>
    public void RequestRespawn()
    {
        if (Health <= 0) Respawn();
    }

    /// <summary>
    /// Attempts to activate ability index <paramref name="AbilityIdx"/>
    /// </summary>
    public void TryActivate(int AbilityIdx)
    {
        if (AbilityIdx >= 0 && AbilityIdx < Abilities.Length && Abilities[AbilityIdx].TryDo(this))
        {
            ActivatedAbilityEvent?.Invoke(AbilityIdx);
        }
    }

    private void ApplyDamage(float dmg)
    {
        if (Health < 0) return;
        Health -= dmg / (Math.Max(0, GetDefense()) + 1);
        if (Health <= 0) DiedEvent?.Invoke();
    }


    //* Updates

    public CharacterUpdateDTO GetUpdate(CharacterAttribute type)
    {
        byte[]? payload = null;
        switch (type)
        {
            case CharacterAttribute.Position:
                payload = Serialization.Serialize(Position);
                break;
            case CharacterAttribute.Health:
                payload = Serialization.Serialize(Health);
                break;
            case CharacterAttribute.Direction:
                payload = Serialization.Serialize(Direction);
                break;
            case CharacterAttribute.State:
                payload = Serialization.Serialize(State);
                break;
            case CharacterAttribute.PrimarySlot:
                payload = Serialization.Serialize(_primarySlot.EquipmentEnum);
                break;
        }
        if (payload is null) throw new Exception();
        return new CharacterUpdateDTO(type, payload, ID);
    }
    public PlayerUpdateDTO GetUpdate(PlayerAttribute type)
    {
        byte[]? payload = null;
        switch (type)
        {
            case PlayerAttribute.Logistics:
                payload = Serialization.Serialize(Logistics);
                break;
        }
        if (payload is null) throw new Exception();
        return new PlayerUpdateDTO(type, payload);
    }

    /// <summary>
    /// Polls updates that should be shown to all players
    /// </summary>
    public IEnumerable<AbstractUpdateDTO> PollUpdates()
    {
        if (_health.PollChanged())      yield return GetUpdate(CharacterAttribute.Health);
        if (_direction.PollChanged())   yield return GetUpdate(CharacterAttribute.Direction);
        if (_state.PollChanged())       yield return GetUpdate(CharacterAttribute.State);

        if (_primarySlot.PollEquipmentUpdate()) yield return GetUpdate(CharacterAttribute.PrimarySlot);
    }
    /// <summary>
    /// Updates that should only be seen by the player associated with this character
    /// </summary>
    public IEnumerable<AbstractUpdateDTO> PollPlayerUpdates()
    {
        if (_logistics.PollChanged()) yield return GetUpdate(PlayerAttribute.Logistics);
        foreach( var kvp in _primarySlot.PollUpdates() ) yield return new FirearmSlotUpdateDTO(kvp.Key, kvp.Value);
    }

    public void SetBuildTarget(int x, int y, TileType tile)
    {
        BuildTarget = tile;
        BuildCell = new(x, y);

        State = CharacterState.Building;
    }

    internal Vector2I GetCell()
    {
        return new((int)(Position.X / CommonDefines.CellSize), (int)(Position.Z / CommonDefines.CellSize));
    }

    public void TryReload()
    {
        if (_primarySlot.TryReload()) ReloadEvent?.Invoke(this);
    }

    /// <summary>
    /// Attempts to buy <paramref name="purchaseRequest"/>, if enough logistics
    /// </summary>
    /// <param name="purchaseRequest"></param>
    public void TryPurchase(FirearmEnum equipmentEnum)
    {
        if (equipmentEnum != PrimarySlot.EquipmentEnum)
        {
            FirearmType equipment = EquipmentTypes.Get(equipmentEnum);
            
            if (Logistics < equipment.LogisticsCost) return;
            Logistics -= equipment.LogisticsCost;

            _primarySlot.Equipment = equipment;

        }
    }
}