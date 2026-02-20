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

namespace OpenTrenches.Core.Scene;

[GlobalClass]
public partial class ClientRoot : Node
{
    //* Networking
    private IClientNetworkAdapter NetworkAdapter { get; }
    private ClientNetworkHandler? ClientNetworkHandler;

    private readonly ConnectionAgent ConnectionAgent;

    //* GD
    private WorldView World { get; set; } = default!;

    private KeyboardListener KeyboardListener { get; }

    //* State
    private ClientState? State { get; set; }

    //* GUI
    private CharacterControlUi _characterUI = null!; 

    private DeathScreen _deathScreen = null!;

    public ClientRoot()
    {
        //* Network

        NetworkAdapter = new LiteNetClientAdapter();
        NetworkAdapter.Start();

        ConnectionAgent = new();
        ConnectionAgent.ReceivedServersEvent += UpdateServerList;

        // Nodes

        KeyboardListener = new();
        AddChild(KeyboardListener);
    }




    //* Initialize godot items
    public override void _Ready()
    {
        _characterUI = GetNode<CharacterControlUi>("CharacterControlUi");
        _deathScreen = GetNode<DeathScreen>("DeathScreen");
        _deathScreen.Hide();
        // Send player respawn request to server when requested
        _deathScreen.OnRespawnClicked += HandleRespawnAttempt;

        //* Try to connect to server
        ConnectionAgent.PollRecords();
    }

    //* handling user input

    /// <summary>
    /// Sends a respawn attempt to the server.
    /// </summary>
    public void HandleRespawnAttempt() => ClientNetworkHandler?.Adapter.Send(new RespawnCommandRequest());


    //* Changing state

    /// <summary>
    /// Attempts to connect to the game server at <paramref name="endPoint"/>
    /// </summary>
    private async void TryJoin(IPEndPoint endPoint)
    {
        lock (this)
        {
            ClientNetworkHandler = new(NetworkAdapter.Connect(endPoint));
        }
        SetState(ClientNetworkHandler.State);
    }

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
        State.FireEvent += World.RenderProjectile;

        //* Hook state changes
        State.PlayerCharacterSetEvent += SetPlayer;

        State.PlayerState.OnLogisticsChangedEvent += _characterUI.SetLogistics;


        State.PlayerDeathEvent += _deathScreen.Show;
        State.PlayerRespawnEvent += _deathScreen.Hide;

        //* Initialize values
        if (State.PlayerCharacter is not null) SetPlayer(State.PlayerCharacter);
        _characterUI.SetLogistics(State.PlayerState.Logistics);
        
    }
    private void SetWorld(ClientState state)
    {
        World = new(state);
        World.DisablePhysics();
        AddChild(World);
    }
    private void SetPlayer(Character playerCharacter)
    {
        World.AddPlayerComponents(playerCharacter);
        _characterUI.SetPlayer(playerCharacter);
        KeyboardListener.SetPlayer(playerCharacter);
    }


    //* Network

    /// <summary>
    /// Informs the user that they can connect to <paramref name="servers"/>
    /// </summary>
    /// <param name="obj"></param>
    private void UpdateServerList(ServerRecord[] servers)
    {
        //TODO make available to player
        if (servers.Length > 0) TryJoin(servers[0].EndPoint);
    }

    /// <summary>
    /// Polls outgoing user commands from nodes that can produce them
    /// </summary>
    private IEnumerable<AbstractCommandDTO> PollCommands()
        => KeyboardListener.PollCommands()
        .Concat(_characterUI.PollCommands());

    public override void _Process(double delta)
    {
        NetworkAdapter.Poll();

        if (ClientNetworkHandler is not null)
        {
            // Stream user key and mouse input
            ClientNetworkHandler.Adapter.Send(KeyboardListener.GetStatus());
            foreach (AbstractCommandDTO cmd in PollCommands()) ClientNetworkHandler.Adapter.Send(cmd);
        }

    }
}