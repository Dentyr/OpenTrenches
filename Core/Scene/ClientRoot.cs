using Godot;
using OpenTrenches.Core.Scene.World;
using OpenTrenches.Common.Multiplayer;
using OpenTrenches.Core.Scripting;
using OpenTrenches.Core.Scripting.Adapter;
using OpenTrenches.Common.Contracts.DTO;

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
        if (ClientNetworkHandler.State is not null) LoadGame(ClientNetworkHandler.State);
    }

    private void LoadGame(ClientState state) //TODO load when server decides on new game
    {
        State = state;
        SetWorld(state);

        State.CharacterAddedEvent += World.AddCharacter;


        State.PlayerCharacterSetEvent += World.AddPlayerComponents;
        State.PlayerCharacterSetEvent += KeyboardListener.SetPlayer;
        State.FireEvent += World.RenderProjectile;
    }
    private void SetWorld(ClientState state)
    {
        World = new(state);
        World.DisablePhysics();
        AddChild(World);
    }


    public override void _Process(double delta)
    {
        NetworkAdapter.Poll();

        // Stream user key and mouse input
        ClientNetworkHandler.Adapter.Send(KeyboardListener.GetStatus());
        foreach (AbstractCommandDTO cmd in KeyboardListener.PollCommands()) ClientNetworkHandler.Adapter.Send(cmd);
    }
}