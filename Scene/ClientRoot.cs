using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Godot;
using LiteNetLib;
using OpenTrenches.Scene.World;
using OpenTrenches.Scripting.Multiplayer;

namespace OpenTrenches.Scene;

[GlobalClass]
public partial class ClientRoot : Node
{
    private IClientNetworkAdapter NetworkAdapter { get; }
    private INetworkConnectionAdapter Connection { get; }
    private WorldNode? World { get; set; }

    KeyboardListener keyboardListener;

    public ClientRoot()
    {
        NetworkAdapter = new LiteNetClientAdapter();
        NetworkAdapter.Start();
        Connection = NetworkAdapter.Connect("localhost");

        keyboardListener = new();
        AddChild(keyboardListener);
    }
    public override void _Ready()
    {
        World = GetNode<WorldNode>("World");
        World.DisablePhysics();
    }
    public override void _Process(double delta)
    {
        NetworkAdapter.Poll();
        Connection.Stream(Serialization.Serialize(keyboardListener.GetStatus()));
    }
    
}