
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
using OpenTrenches.Core.Scripting.World;
using OpenTrenches.Server.Scripting.Ability;
using OpenTrenches.Server.Scripting.Adapter;
using OpenTrenches.Server.Scripting.Combat;
using OpenTrenches.Server.Scripting.Teams;
using OpenTrenches.Server.Scripting.World;
namespace OpenTrenches.Server.Scripting.Player;

public class Character : IIdObject, IWorldObject
{
    private const int LogisticsBountyBase = 1;
    /// <summary>
    /// Multiplier to the equipment valaue of a character to give to its killer
    /// </summary>
    private const float LogisticsValueBountyMult = 0.5f;
    /// <summary>
    /// Multiplier to the seconds alive that should be granted as logistics ot the killer
    /// </summary>
    private const float AliveValueBountyMult = 1f / 15f; // 1 point every 15 seconds

    //* Identification
    private IServerState ServerState { get; }
    public int ID { get; }
    public Team Team { get; }

    //* State in World
    
    private readonly UpdateableProperty<Vector2> _position;
    public Vector2 Position
    {
        get => _position.Value;
        set => _position.Value = value;
    }


    private readonly UpdateableProperty<WorldLayer> _layer;
    /// <summary>
    /// Where this character exists in relation to possible movement
    /// </summary>
    public WorldLayer Layer
    {
        get => _layer.Value;
        private set 
        {
            if (_layer.Set(value)) LayerChangedEvent?.Invoke(value);
        }
    }
    public event Action<WorldLayer>? LayerChangedEvent;


    private readonly UpdateableProperty<Vector2> _direction;
    /// <summary>
    /// The location this character is looking towards
    /// </summary>
    /// <value></value>
    public Vector2 Direction
    {
        get => _direction.Value;
        set => _direction.Value = value;
    }

    /// <summary>
    /// Cardinal velocity
    /// </summary>
    public Vector2 MovementVelocity { get; private set; } = Vector2.Zero;
    /// <summary>
    /// Sets movement velocity in <paramref name="direction"/>
    /// </summary>
    public void MoveIn(Vector2 direction)
    {
        MovementVelocity = direction.Normalized() * 7f;
    }
    public void StopMoving() => MovementVelocity = Vector2.Zero;

    private UpdateableProperty<CharacterState> _state; 
    public CharacterState State
    { 
        get => _state.Value;
        set 
        {
            if (_state.Set(value)) StateChangedEvent?.Invoke(_state);
        }
    }
    public event Action<CharacterState>? StateChangedEvent;

    //* Combat status
    private readonly UpdateableProperty<float> _health;
    public float Hp 
    { 
        get => _health;
        private set => _health.Value = value;
    }

    /// <summary>
    /// Once the server tick has surpassed this value, this character can respawn
    /// </summary>
    private long _allowRespawnTick = 0;

    private long _tickRespawned = 0;

    private readonly UpdateableProperty<int> _logistics;
    public int Logistics
    {
        get => _logistics.Value;
        private set => _logistics.Value = value;
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


    public event Action<Character, Vector2>? FireEvent;
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

    //* Update events
    /// <summary>
    /// Update for the character's state, publicly visible
    /// </summary>
    public event Action<AbstractUpdateDTO>? CharacterUpdateEvent;
    /// <summary>
    /// Update for the player's state, should only be visible to controller
    /// </summary>
    public event Action<AbstractUpdateDTO>? PlayerUpdateEvent;

    private void PropagateUpdate<T>(CharacterAttribute type, T value)
        => CharacterUpdateEvent?.Invoke(new CharacterUpdateDTO(type, Serialization.Serialize(value), ID));

    private void PropagateUpdate<T>(PlayerAttribute type, T value)
        => PlayerUpdateEvent?.Invoke(new PlayerUpdateDTO(type, Serialization.Serialize(value)));

    private void PropagateUpdate(FirearmSlotAttribute type, byte[] payload)
        => PlayerUpdateEvent?.Invoke(new FirearmSlotUpdateDTO(type, payload));

    public Character(IServerState ServerState, int ID, Team Team)
    {
        this.ServerState = ServerState;
        this.ID = ID;
        this.Team = Team;

        //* Initializing shared props
        _position = new(x => PropagateUpdate(CharacterAttribute.Position, x));
        _direction = new(x => PropagateUpdate(CharacterAttribute.Direction, x));
        _state = new(0, x => PropagateUpdate(CharacterAttribute.State, x));
        _health = new(x => PropagateUpdate(CharacterAttribute.Health, x));

        _layer = new(x => PropagateUpdate(CharacterAttribute.Layer, x));


        _primarySlot.EquipmentUpdateEvent += x => PropagateUpdate(CharacterAttribute.PrimarySlot, x);

        //* Initializing player props
        _primarySlot.AttributeChangedEvent += PropagateUpdate;
        _logistics = new(100, x => PropagateUpdate(PlayerAttribute.Logistics, x));

        Respawn(Team.Camps.FirstOrDefault());
    }

    //* Simulation

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
        foreach (var abiltiy in Abilities) abiltiy.ProgressTimer(delta);
        _primarySlot.Cooldown(delta, State.HasFlag(CharacterState.Aiming));
        


        //* building
        if (State.HasFlag(CharacterState.Shooting))
        {
            TryFire(adapter);
        }
        if (State.HasFlag(CharacterState.Building))
        {
            if (Position.DistanceTo(BuildCell.CellToPosition()) > 1f) 
            {
                CancelTasks();
                return;
            }

            // only proceed if position is valid
            if (ServerState.Chunks.TryGetCell(BuildCell, out CellRecord? cell))
            {
                if (cell.Tile == TileType.Clear && cell.TileConstruction == null)
                {   //if no tile exists at position, make one with a build status.
                    ServerState.Chunks.StartBuild(BuildCell, BuildTarget, delta);
                }
                else if (cell.Tile == BuildTarget)
                    CancelTasks(); // already constructed, no further action
                else if (cell.TileConstruction is TileConstruction status && status.Target == BuildTarget) 
                {   // If build status is not null, make progress on building status if the target is the same
                    var construct = ServerState.Chunks.ProgressBuild(BuildCell, delta);
                    if (construct?.Progress >= 1) CancelTasks();
                }
                else if (cell.TileConstruction is null)
                {   // if a tile exists, isn't the build target, and can be built, 
                    ServerState.Chunks.StartBuild(BuildCell, BuildTarget);
                }
                else CancelTasks();
            }

            else CancelTasks();
        }
        // Jump out of trench
        if (State.HasFlag(CharacterState.Jumping))
        {
            TryClear(CharacterState.Jumping);
            //Must be in trench
            if (Layer == WorldLayer.Trench)
            {
                Vector2? target = adapter.AdaptJump(MovementVelocity);

                // can't move up because blocked or no ledge
                if (target is not null)
                {
                    Layer = WorldLayer.Ground;
                    Position = (Vector2)target;
                }
            }
        }

        //* Position changes
        // Drop to lower level if on ground level, but current cell is trench level
        if (Layer == WorldLayer.Ground
            && ServerState.Chunks.TryGetTile(GetCell(), out TileType? current) 
            && current == TileType.Trench)
        {
            Layer = WorldLayer.Trench;
        }
    }
    private void TryFire(ICharacterAdapter adapter)
    {
        if (Position != Direction 
            && _primarySlot.CanFire()
        ) {
            // human recoil spread
            FirearmType firearm = _primarySlot.Equipment;

            // Vector going in the direction of the bullet
            var aimVector = 
                (Direction - Position)
                    .Spread(_primarySlot.Recoil)
                    .Normalized();
            for (int i = 0; i < firearm.Stats.ProjectilesPerShot; i ++)
            {
                
                // machine spread
                var kineticTarget = aimVector.Spread(firearm.Stats.SpreadMOA) * firearm.Stats.ProjectileDistance;

                WorldLayer fireLayer = Layer;

                // If the character is inside a trench and aiming, then if they're aiming at a ground tile (above the trench) or aiming at a trench, but one that's blocked by ground, they will shoot out from the trench
                if (fireLayer == WorldLayer.Trench && 
                    State.HasFlag(CharacterState.Aiming) && 
                    (
                        GetTargetLayer() == WorldLayer.Ground ||
                        TileArrayGeometryService.LineContainsTile(ServerState.Chunks, Position, Direction, TileType.Clear)
                    )
                    )
                {
                    fireLayer = WorldLayer.Ground;
                }
                
                FireHitResult result = adapter.AdaptFire(fireLayer, Position + kineticTarget);
                if (result is FireHitResult.HitCharacter hit && hit.Character.Team != Team)
                {
                    // if dealt killing blow, add logistics
                    if (hit.Character.ApplyDamage(_primarySlot.Equipment.Stats.DamagePerProjectile))
                    {
                        Logistics += hit.Character.GetLogisticsBounty();
                    }
                }
                else if (result is FireHitResult.HitStructure hitStruct && hitStruct.Structure.Team != Team) hitStruct.Structure.ApplyDamage(_primarySlot.Equipment.Stats.DamagePerProjectile);
                
                FireEvent?.Invoke(this, result.Position);
            }
            // Apply recoil
            _primarySlot.UseBullet();
            _primarySlot.ApplyCooldownAndRecoil();
        }
        else TryClear(CharacterState.Shooting);
    }


    /// <summary>
    /// Stops combat actions like shooting and aiming
    /// </summary>
    public void CancelAttack()
    {
        State &= ~CharacterState.Aiming;
        State &= ~CharacterState.Shooting;
    }
    /// <summary>
    /// Stops every state action this character is doing
    /// </summary>
    public void CancelTasks()
    {
        State = 0;
    }

    //TODO hide member and expose functions for specific members
    public void TrySet(CharacterState flag)
    {
        switch (flag)
        {
            case CharacterState.Aiming:
                break;
            case CharacterState.Shooting:
                if (!_primarySlot.CanFire()) return;
                break;
            case CharacterState.Building:
                return; // do not set building from here
            case CharacterState.Jumping:
                break;
        }
        State |= flag;
    }
    public void TryClear(CharacterState flag)
    {
        State &= ~flag;
    }

    private int GetLogisticsBounty()
    {
        int bountyAlive = (int)((ServerState.ServerTick - this._tickRespawned) / Engine.PhysicsTicksPerSecond * AliveValueBountyMult);
        int bountyEquipment = (int)((PrimarySlot.Equipment?.LogisticsCost ?? 0) * LogisticsValueBountyMult);
        return LogisticsBountyBase + bountyAlive + bountyEquipment;
    }
    //* combat

    /// <summary>
    /// Returns the <see cref="WorldLayer"/> of the tile at <see cref="Direction"/>
    /// </summary>
    /// <returns></returns>
    public WorldLayer GetTargetLayer()
    {
        if (ServerState.Chunks.TryGetTile((int)Direction.X, (int)Direction.Y, out TileType? tile))
            return TileLayerConversion.LayerOf(tile);
        
        return WorldLayer.Ground;
    }


    
    /// <summary>
    /// Sets character back to full health and spawns them around <paramref name="structure"/>
    /// </summary>
    public void Respawn(ServerStructure? structure)
    {
        Hp = CommonDefines.MaxHp; 

        // spawn near structure
        if (structure is not null)
        {
            Vector2 offset = Vector2.Zero;
            if (StructureTypes.TryGet(structure.Enum, out var type))
                offset = type.Profile.Size + type.Profile.Position;

            Position = structure.Position.CellToPosition() + offset;
        }
        else Position = Team.DefaultSpawnPoint;

        // clear timers

        foreach (var ability in Abilities) ability.ClearTimer();
        Layer = WorldLayer.Ground;
        _primarySlot.ResetState();

        _tickRespawned = ServerState.ServerTick;

        // clear equipment
        _primarySlot.Equipment = EquipmentTypes.Get(FirearmEnum.Rifle);

        RespawnEvent?.Invoke();
    }

    /// <summary>
    /// Attempt by the character to respawn.
    /// </summary>
    public void RequestRespawn(int campId)
    {
        // If character is dead and respawn timer is finished, will respawn if camp is of the same team and alive.
        if (Hp <= 0 && 
            ServerState.ServerTick >= _allowRespawnTick &&
            ServerState.Chunks.StructureDict.TryGetValue(campId, out var camp) &&
            camp.Team == Team &&
            camp.Enum == StructureEnum.Camp &&
            !camp.Destroyed
        ) {
            Respawn(camp);
        }
    }

    /// <summary>
    /// Attempts to activate ability index <paramref name="AbilityIdx"/>
    /// </summary>
    public void TryActivate(int AbilityIdx)
    {
        if (AbilityIdx < 0 || AbilityIdx >= Abilities.Length) return;

        var ability = Abilities[AbilityIdx];

        if (Logistics < ability.Record.Cost) return;

        if (ability.TryDo(this))
        {
            Logistics -= ability.Record.Cost;
            ActivatedAbilityEvent?.Invoke(AbilityIdx);
        }
    }

    /// <summary>
    /// Returns true if dealing <paramref name="dmg"/> is the killing blow
    /// </summary>
    private bool ApplyDamage(float dmg)
    {
        if (Hp <= 0) return false;
        Hp -= dmg / (Math.Max(0, GetDefense()) + 1);
        if (Hp <= 0)
        {
            _allowRespawnTick = ServerState.ServerTick + (long)(Engine.PhysicsTicksPerSecond * CommonDefines.SecondsForCharacterRespawn);
            DiedEvent?.Invoke();
            return true;
        }
        return false;
    }


    //* Updates


    public void SetBuildTarget(int x, int y, TileType tile)
    {
        BuildTarget = tile;
        BuildCell = new(x, y);

        State = CharacterState.Building;
    }

    internal Vector2I GetCell()
    {
        return new((int)(Position.X), (int)(Position.Y));
    }

    public void TryReload()
    {
        if (_primarySlot.Equipment is not FirearmType firearm) return;

        int reloadCost = firearm.Stats.ReloadLogisticsCost;
        if (Logistics < reloadCost) return;

        if (_primarySlot.TryReload())
        {
            Logistics -= reloadCost;
            ReloadEvent?.Invoke(this);
        }
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

    /// <summary>
    /// Attempts to leave a trench in the current facing direction
    /// </summary>
    public void TryExitTrench()
    {
        State = CharacterState.Jumping;
    }
}
