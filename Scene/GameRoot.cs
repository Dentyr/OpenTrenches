using System;
using System.Collections.Generic;
using System.Diagnostics;
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

    private ServerState GameState { get; } = new();

    public GameRoot()
    {
        Console.WriteLine($"PID {Process.GetCurrentProcess().ProcessName}");
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
    public override void _Ready()
    {
        World = GetNode<WorldNode>("World");
        GameState.CharacterAddedEvent += World.AddCharacter;
        GameState.CharacterAddedEvent += BroadcastCharacter;
    }
    private void BroadcastCharacter(ushort id, Character character) 
        => NetworkAdapter.Send(new CreateDatagram(ObjectCategory.Character, id, Serialization.Serialize(character)));


    private void Connection(INetworkConnectionAdapter connection)
    {
        PlayerNetworkHandler player = new(connection, GameState);
        _players.Add(player);
        foreach (var character in GameState.Characters) player.Adapter.Message(new CreateDatagram(ObjectCategory.Character, character.Key, Serialization.Serialize<Character>(character.Value)));
        player.Adapter.Send(new MessageDatagram(MessageCategory.Setplayer, Serialization.Serialize<ushort>(player.CharacterId)));
        
    }




    public override void _Process(double delta)
    {
        NetworkAdapter.Poll();
        foreach(var playerKvp in GameState.Characters)
            NetworkAdapter.StreamBroadcast(new UpdateDatagram(ObjectCategory.Character, playerKvp.Key, playerKvp.Value.GetUpdate(Character.UpdateType.Position)));
    }
}
