using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Godot;
using LiteNetLib;
using LiteNetLib.Utils;
using OpenTrenches.Scene.World;
using OpenTrenches.Scripting;
using OpenTrenches.Scripting.Datastream;
using OpenTrenches.Scripting.Multiplayer;
using OpenTrenches.Scripting.Player;

namespace OpenTrenches.Scene;

[GlobalClass]
public partial class GameRoot : Node
{
    public IServerNetworkAdapter NetworkAdapter;
    public WorldNode World = null!;

    private List<PlayerNetworkHandler> _players = [];
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
        PlayerNetworkHandler player = new(adapter);
        _players.Add(player);
        World.AddChild(new CharacterNode3D(player.Character));
        adapter.Message(new CreateDatagram(ObjectCategory.Character, 0, Serialization.Serialize(player.Character)));
    }

    public override void _Ready()
    {
        World = GetNode<WorldNode>("World");
    }



    public override void _Process(double delta)
    {
        NetworkAdapter.Poll();
        foreach(var player in _players.Select(x => x.Character)) 
            NetworkAdapter.StreamBroadcast(new UpdateDatagram(ObjectCategory.Character, 0, player.GetUpdate(Character.UpdateType.Position)));
    }
}
