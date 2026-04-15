using Godot;
using OpenTrenches.Core.Scene.World;
using OpenTrenches.Common.Multiplayer;
using OpenTrenches.Core.Scripting;
using OpenTrenches.Core.Scripting.Adapter;
using OpenTrenches.Common.Contracts.DTO;
using OpenTrenches.Core.Scene.GUI;
using OpenTrenches.Core.Scene.Gui;
using System.Linq;
using System.Collections.Generic;
using System;
using OpenTrenches.Core.Scripting.Player;
using OpenTrenches.Common.Contracts.DTO.ServerComands;
using OpenTrenches.Common.Contracts.DTO.PlayerCommands;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using OpenTrenches.Server.Scene;

namespace OpenTrenches.Core.Scene;

[GlobalClass]
public partial class ClientRoot : Node
{
    ClientNetworkManager ClientNetworkManager;
    
    //* GD
    private WorldView World { get; set; } = default!;

    private KeyboardListener KeyboardListener { get; }

    //* GUI
    private CharacterControlUi _characterUI = null!; 

    private DeathScreen _deathScreen = null!;

    private GameEndScreen _gameEndScreen = null!;


    //* State
    private ClientState? State { get; set; }


    public ClientRoot()
    {
        ClientNetworkManager = new();
        ClientNetworkManager.JoinGameEvent += SetState;

        // Nodes

        KeyboardListener = new();
        AddChild(KeyboardListener);
    }




    //* Initialize godot items
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

        //* Try to connect to server
        ClientNetworkManager.PollAvailableServers();
    }

    //* handling user input

    /// <summary>
    /// Sends a respawn attempt to the server.
    /// </summary>
    public void HandleRespawnAttempt() => ClientNetworkManager.Send(new RespawnCommandRequest());

    public void HandleReturnAttempt() => GD.Print(" not yet implemented "); //TODO implement main menu screen


    //* Changing state


    /// <summary>
    /// Renders <paramref name="state"/>
    /// </summary>
    private void SetState(ClientState state) //TODO load when server decides on new game
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
        if (State.PlayerCharacterId is uint notnull && State.TryGetCharacter(notnull, out var player)) SetPlayer(new LocalPlayerView(player, State.PlayerState));
        _characterUI.SetLogistics(State.PlayerState.Logistics);
        
    }
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


    //* Network


    /// <summary>
    /// Polls outgoing user commands from nodes that can produce them
    /// </summary>
    private IEnumerable<AbstractCommandDTO> PollCommands()
        => KeyboardListener.PollCommands()
        .Concat(_characterUI.PollCommands());

    public override void _Process(double delta)
    {
        ClientNetworkManager.Poll();

        if (ClientNetworkManager.CanSend)
        {
            // Stream user key and mouse input
            ClientNetworkManager.Send(KeyboardListener.GetStatus());
            foreach (AbstractCommandDTO cmd in PollCommands()) ClientNetworkManager.Send(cmd);
        }
    }
}