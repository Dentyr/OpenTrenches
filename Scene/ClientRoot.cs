using Godot;
using OpenTrenches.Core.Scene.World;
using OpenTrenches.Common.Multiplayer;
using OpenTrenches.Core.Scripting;
using OpenTrenches.Core.Scripting.Adapter;

namespace OpenTrenches.Core.Scene;

[GlobalClass]
public partial class ClientRoot : Node
{
    //* Networking
    private IClientNetworkAdapter NetworkAdapter { get; }
    private ClientNetworkHandler ClientNetworkHandler { get; }

    //* GD
    private WorldNode? World { get; set; }

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
        World = GetNode<WorldNode>("World");

        State = ClientNetworkHandler.State;
        if (State is not null)
        {
            State.CharacterAddedEvent += World.AddCharacter;
            State.ChunkSetEvent += World.AddChunk;


            State.PlayerCharacterSetEvent += World.AddPlayerComponents;
            State.PlayerCharacterSetEvent += KeyboardListener.SetPlayer;
            State.FireEvent += World.RenderProjectile;
        }

        World.DisablePhysics();
    }
    public override void _Process(double delta)
    {
        NetworkAdapter.Poll();

        // Stream user key and mouse input
        ClientNetworkHandler.Adapter.Send(KeyboardListener.GetStatus());
    }
}