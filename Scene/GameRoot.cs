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
        World.AddChild(new CharacterNode3D(player.Character));

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
        Vector3 movement = Vector3.Zero;
        foreach (UserKey key in input.Keys)
        {
            switch(key)
            {
                case UserKey.W:
                movement.X += 1;
                break;
                case UserKey.A:
                movement.Z -= 1;
                break;
                case UserKey.S:
                movement.X -= 1;
                break;
                case UserKey.D:
                movement.Z += 1;
                break;
            }
        }
        movement *= 250f;
        Character.Movement = movement;
    }
}