using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Godot;
using LiteNetLib;
using OpenTrenches.Scripting.Multiplayer;

namespace OpenTrenches.Scene;

[GlobalClass]
public partial class ClientRoot : Node
{
    IClientNetworkAdapter networkAdapter = new LiteNetClientAdapter();
    public ClientRoot()
    {
        networkAdapter.Start();
        networkAdapter.Connect("localhost");

    }
    public override void _Process(double delta)
    {
        networkAdapter.Poll();
    }
    
}