using System;
using System.Linq;
using System.Threading;
using Godot;
using LiteNetLib;
using LiteNetLib.Utils;
using OpenTrenches.Scene.World;
using OpenTrenches.Scripting;
using OpenTrenches.Scripting.Multiplayer;

namespace OpenTrenches.Scene;

[GlobalClass]
public partial class GameRoot : Node
{
    public IServerNetworkAdapter NetworkAdapter;
    public WorldNode World = null!;
    public GameRoot()
    {
        NetworkAdapter = new LiteNetServerAdapter();
        NetworkAdapter.Start();
        NetworkAdapter.ConnectedEvent += Connection;
        // while (!Console.KeyAvailable)
        // {
        //     networkAdapter.Poll();
        //     Thread.Sleep(15);
        // }
        // adapter.Connect(new IPEndPoint(IPAddress.Loopback, NetworkDefines.ServerPort));
        // IServerAdapter serverInstance = new SocketAdapter();
        // adapter.Listen();
    }
    private void Connection(INetworkConnectionAdapter adapter)
    {
        PlayerConnection player = new(adapter);
    }

    public override void _Ready()
    {
        World = GetNode<WorldNode>("World");
    }



    public override void _Process(double delta)
    {
        NetworkAdapter.Poll();
    }
}


public class PlayerConnection
{
    public INetworkConnectionAdapter Adapter { get; }
    public Character Character { get; }


    public PlayerConnection(INetworkConnectionAdapter Adapter)
    {
        this.Adapter = Adapter;
        Adapter.ReceiveEvent += HandleInput;
        Character = new();
    }

    private void HandleInput(byte[] packet)
    {
        var input = Serialization.Deserialize<InputStatus>(packet);
        foreach (UserKey key in input.Keys)
        {
            Vector3 movement = Vector3.Zero;
            switch(key)
            {
                case UserKey.W:
                Console.WriteLine("W");
                movement.Y -= 5;
                break;
                case UserKey.A:
                break;
                case UserKey.S:
                break;
                case UserKey.D:
                break;
            }
            Character.Movement += movement;
        }
    }
}