using System;
using System.Threading;
using Godot;
using LiteNetLib;
using LiteNetLib.Utils;
using OpenTrenches.Scripting;
using OpenTrenches.Scripting.Multiplayer;

namespace OpenTrenches.Scene;

[GlobalClass]
public partial class GameRoot : Node
{
    IServerNetworkAdapter networkAdapter;
    public GameRoot()
    {
        networkAdapter = new LiteNetServerAdapter();
        networkAdapter.Start();
        // while (!Console.KeyAvailable)
        // {
        //     networkAdapter.Poll();
        //     Thread.Sleep(15);
        // }
        // adapter.Connect(new IPEndPoint(IPAddress.Loopback, NetworkDefines.ServerPort));
        // IServerAdapter serverInstance = new SocketAdapter();
        // adapter.Listen();
    }
    public override void _Process(double delta)
    {
        networkAdapter.Poll();
    }
}