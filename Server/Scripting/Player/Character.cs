
// namespace 
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using OpenTrenches.Common.Ability;
using OpenTrenches.Common.Contracts;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Contracts.DTO;
using OpenTrenches.Common.World;
using OpenTrenches.Server.Scripting.Ability;
using OpenTrenches.Server.Scripting.Adapter;
namespace OpenTrenches.Server.Scripting.Player;

public class Character(IServerState ServerState, ushort ID, int Team) : IIdObject
{
    //* Identification
    private IServerState ServerState { get; } = ServerState;
    public ushort ID { get; } = ID;
    public int Team { get; } = Team;

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
    private readonly UpdateableProperty<float> _health = new(10);
    public float Health 
    { 
        get => _health;
        private set => _health.Value = value;
    }

    //TODO Update stats to be more modular

    private float _baseDefense { get; set; } = 0;
    private float GetDefense()
    {

        return _baseDefense + Abilities.Sum(x => x.Active ? x.Record.DefenseMod : 0);
    }

    private CharacterAbility[] Abilities { get; } = [new(AbilityRecords.StimulantAbility)];
    public event Action<int>? ActivatedAbilityEvent;


    private readonly UpdateableProperty<float> _stateCooldown = new(0);
    public float StateCooldown
    { 
        get => _stateCooldown;
        private set => _stateCooldown.Value = value;
    }
    public event Action<Character, Vector3>? FireEvent;


    //* build target

    public TileType BuildTarget { get; private set; }
    public Vector2I BuildCell { get; private set; }



    /// <summary>
    /// Called when the adapter simulates time passing
    /// </summary>
    /// <param name="delta"></param>
    /// <param name="adapter"></param>
    public void AdapterSimulate(float delta, ICharacterAdapter adapter)
    {
        // Ability usage
        foreach (var abiltiy in this.Abilities) abiltiy.ProgressTimer(delta);


        StateCooldown -= delta;
        

        if (StateCooldown < 0) 
        {
            StateCooldown = 0;

            //* shooting
            
            if (State == CharacterState.Shooting)
            {
                if (Position != Direction)
                {
                    var target = Position + ((Direction - Position).Normalized() * 1000);
                    FireHitResult result = adapter.AdaptFire(target);
                    if (result is FireHitResult.Hit hit) hit.Character.ApplyDamage(4);
                    
                    FireEvent?.Invoke(this, result.Position);
                    StateCooldown = 0.1f;
                }
            }
        }

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

    public void TryActivate(int AbilityIdx)
    {
        if (AbilityIdx >= 0 && AbilityIdx < Abilities.Length && Abilities[AbilityIdx].TryDo(this))
        {
            ActivatedAbilityEvent?.Invoke(AbilityIdx);
        }
    }

    private void ApplyDamage(float dmg)
    {
        Health -= dmg / (Math.Max(0, GetDefense()) + 1);
    }


    //* build
    private void StartBuild()
    {
        ServerState.Chunks.StartBuild(BuildCell, BuildTarget);
    }


    //* Updates

    public AbstractUpdateDTO GetUpdate(CharacterAttribute type)
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
            case CharacterAttribute.Cooldown:
                payload = Serialization.Serialize(StateCooldown);
                break;
            case CharacterAttribute.State:
                payload = Serialization.Serialize(State);
                break;
        }
        if (payload is null) throw new Exception();
        return new CharacterUpdateDTO(type, payload, ID);
    }

    public IEnumerable<AbstractUpdateDTO> PollUpdates()
    {
        if (_health.PollChanged())      yield return GetUpdate(CharacterAttribute.Health);
        if (_direction.PollChanged())   yield return GetUpdate(CharacterAttribute.Direction);
        if (_state.PollChanged())       yield return GetUpdate(CharacterAttribute.State);
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
}