using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Threading;
using Godot;
using LiteNetLib;
using OpenTrenches.Scene.World;
using OpenTrenches.Scripting.Multiplayer;
using OpenTrenches.Scripting.Datastream;

namespace OpenTrenches.Scene;

[GlobalClass]
public partial class ClientRoot : Node
{
    private IClientNetworkAdapter NetworkAdapter { get; }
    private ClientNetworkHandler ClientNetworkHandler { get; }
    private WorldNode? World { get; set; }

    private KeyboardListener KeyboardListener { get; }

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
        ClientNetworkHandler.World = World;
        World.DisablePhysics();
    }
    public override void _Process(double delta)
    {
        NetworkAdapter.Poll();

        // Stream user key and mouse input
        ClientNetworkHandler.Adapter.Stream(new StreamDatagram(StreamCategory.Input, Serialization.Serialize(KeyboardListener.GetStatus())));
    }
}