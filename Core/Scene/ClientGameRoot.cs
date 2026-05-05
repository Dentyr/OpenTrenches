using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using OpenTrenches.Common.Contracts;
using OpenTrenches.Common.Contracts.DTO;
using OpenTrenches.Common.Contracts.DTO.DataModel;
using OpenTrenches.Common.Contracts.DTO.PlayerCommands;
using OpenTrenches.Common.Contracts.DTO.UpdateModel;
using OpenTrenches.Common.Multiplayer;
using OpenTrenches.Core.Scene.Gui;
using OpenTrenches.Core.Scene.GUI;
using OpenTrenches.Core.Scene.World;
using OpenTrenches.Core.Scripting;
using OpenTrenches.Core.Scripting.Adapter;

/// <summary>
/// Coordinates interaction with a game state
/// </summary>
public partial class ClientGameRoot : Node
{
    private ClientState? State { get; set; }

    //* GD
    private WorldView World { get; set; } = default!;

    private KeyboardListener KeyboardListener { get; }

    //* GUI
    private CharacterControlUi _characterUI = null!; 

    private DeathScreen _deathScreen = null!;

    private GameEndScreen _gameEndScreen = null!;

    public event Action<AbstractUpdateDTO>? OutgoingUpdateEvent;
    public event Action<AbstractCreateDTO>? OutgoingCreateEvent;
    public event Action<AbstractCommandDTO>? OutgoingCommandEvent;
    public event Action<AbstractStreamDTO>? OutgoingStreamEvent;

    /// <summary>
    /// Notifies when the user exits the game root
    /// </summary>
    public event Action? ExitGameEvent;
    
    public ClientGameRoot()
    {
        
        // Nodes

        KeyboardListener = new();
        AddChild(KeyboardListener);
    }


    public override void _Ready()
    {
        _characterUI = GetNode<CharacterControlUi>("UICanvas/CharacterControlUi");
        _deathScreen = GetNode<DeathScreen>("UICanvas/DeathScreen");
        _deathScreen.Visible = false;
        // Send player respawn request to server when requested
        _deathScreen.OnRespawnClicked += HandleRespawnAttempt;

        _gameEndScreen = GetNode<GameEndScreen>("UICanvas/GameEndScreen");
        _gameEndScreen.Visible = false;
        _gameEndScreen.ReturnRequestEvent += HandleReturnAttempt;

    }


    /// <summary>
    /// Renders <paramref name="state"/>
    /// </summary>
    public void SetState(ClientState state) //TODO load when server decides on new game
    {
        State = state;
        State.LoadedEvent += LoadState;
        if (State.Loaded) LoadState();
    }
    /// <summary>
    /// Hooks rendering elements to client state
    /// </summary>
    private void LoadState()
    {
        ArgumentNullException.ThrowIfNull(State);

        //* World changes
        SetWorld(State);



        //* Hook rendered events
        State.CharacterAddedEvent += World.AddCharacter;
        State.StructureAddedEvent += World.AddStructure;
        State.FireEvent += World.RenderProjectile;

        //* Hook state changes
        State.PlayerCharacterSetEvent += SetPlayer;

        State.PlayerState.OnLogisticsChangedEvent += _characterUI.SetLogistics;
        State.PlayerReloadEvent += _characterUI.NotifyPlayerReload;
        State.PlayerFireEvent += _characterUI.NotifyPlayerFire;



        State.PlayerDeathEvent += _deathScreen.Show;
        State.PlayerRespawnEvent += _deathScreen.Hide;

        State.GameEndEvent += victor => _gameEndScreen.ShowEnd(victor, State);

        //* Initialize values
        if (State.PlayerCharacterId is uint notnull && State.TryGetCharacter(notnull, out var player)) 
            SetPlayer(new LocalPlayerView(player, State.PlayerState));
        _characterUI.SetLogistics(State.PlayerState.Logistics);
        
    }

    //* handling user input

    /// <summary>
    /// Sends a respawn attempt to the server.
    /// </summary>
    public void HandleRespawnAttempt() => OutgoingCommandEvent?.Invoke(new RespawnCommandRequest());

    public void HandleReturnAttempt() => ExitGameEvent?.Invoke();


    //* Changing state

    private void SetWorld(ClientState state)
    {
        // clean previous world
        if (World is not null)
        {
            var temp = World;
            RemoveChild(World);
            temp.QueueFree();
        }

        World = new(state);
        World.DisablePhysics();
        AddChild(World);
    }
    private void SetPlayer(LocalPlayerView player)
    {
        World.AddPlayerComponents(player.Character);
        _characterUI.SetPlayer(player);
        KeyboardListener.SetPlayer(player.Character);
    }



    /// <summary>
    /// Polls outgoing user commands from nodes that can produce them
    /// </summary>
    public IEnumerable<AbstractCommandDTO> PollCommands()
        => KeyboardListener.PollCommands()
        .Concat(_characterUI.PollCommands());

    public override void _Process(double delta)
    {
        // Stream user key and mouse input
        OutgoingStreamEvent?.Invoke(KeyboardListener.GetStatus());
        foreach (AbstractCommandDTO cmd in PollCommands()) OutgoingCommandEvent?.Invoke(cmd);
    }
}
