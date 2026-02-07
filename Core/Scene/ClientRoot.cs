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

namespace OpenTrenches.Core.Scene;

[GlobalClass]
public partial class ClientRoot : Node
{
    //* Networking
    private IClientNetworkAdapter NetworkAdapter { get; }
    private ClientNetworkHandler ClientNetworkHandler { get; }

    //* GD
    private WorldView World { get; set; } = default!;

    private KeyboardListener KeyboardListener { get; }

    //* State
    private ClientState? State { get; set; }

    //* GUI
    private CharacterControlUi CharacterUI { get; set; } = null!; 

    public ClientRoot()
    {
        NetworkAdapter = new LiteNetClientAdapter();
        NetworkAdapter.Start();
        ClientNetworkHandler = new(NetworkAdapter.Connect("localhost"));

        KeyboardListener = new();
        AddChild(KeyboardListener);
    }
    public override void _Ready()
    {
        CharacterUI = GetNode<CharacterControlUi>("CharacterControlUi");

        if (ClientNetworkHandler.State is not null) SetClient(ClientNetworkHandler.State);
    }

    private void SetClient(ClientState state) //TODO load when server decides on new game
    {
        State = state;
        State.LoadedEvent += LoadState;
        if (State.Loaded) LoadState();
    }
    private void LoadState()
    {
        ArgumentNullException.ThrowIfNull(State);
        SetWorld(State);


        State.CharacterAddedEvent += World.AddCharacter;

        State.PlayerCharacterSetEvent += SetPlayer;
        if (State.PlayerCharacter is not null) SetPlayer(State.PlayerCharacter);

        State.FireEvent += World.RenderProjectile;
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
        CharacterUI.SetPlayer(playerCharacter);
        KeyboardListener.SetPlayer(playerCharacter);
    }


    private IEnumerable<AbstractCommandDTO> PollCommands()
        => KeyboardListener.PollCommands()
        .Concat(CharacterUI.PollCommands());
    public override void _Process(double delta)
    {
        NetworkAdapter.Poll();

        // Stream user key and mouse input
        ClientNetworkHandler.Adapter.Send(KeyboardListener.GetStatus());
        foreach (AbstractCommandDTO cmd in PollCommands()) ClientNetworkHandler.Adapter.Send(cmd);
    }
}