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

        if (ClientNetworkHandler.State is not null) LoadGame(ClientNetworkHandler.State);
    }

    private void LoadGame(ClientState state) //TODO load when server decides on new game
    {
        State = state;
        SetWorld(state);

        State.CharacterAddedEvent += World.AddCharacter;


        State.PlayerCharacterSetEvent += World.AddPlayerComponents;
        State.PlayerCharacterSetEvent += CharacterUI.SetPlayer;
        State.PlayerCharacterSetEvent += KeyboardListener.SetPlayer;

        State.FireEvent += World.RenderProjectile;
    }
    private void SetWorld(ClientState state)
    {
        World = new(state);
        World.DisablePhysics();
        AddChild(World);
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