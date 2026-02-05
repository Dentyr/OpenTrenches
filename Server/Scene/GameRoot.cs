using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;
using OpenTrenches.Server.Scene.World;
using OpenTrenches.Common.Multiplayer;
using OpenTrenches.Server.Scripting.Player;
using OpenTrenches.Common.Contracts;
using OpenTrenches.Common.Contracts.DTO;
using OpenTrenches.Server.Scripting.Adapter;
using OpenTrenches.Common.World;

namespace OpenTrenches.Server.Scene;

[GlobalClass]
public partial class GameRoot : Node
{
    public IServerNetworkAdapter NetworkAdapter;
    public WorldNode World = null!;

    private readonly List<PlayerNetworkHandler> _players = [];

    private ServerState GameState { get; } = new();

    public GameRoot()
    {
        Console.WriteLine($"PID {Process.GetCurrentProcess().ProcessName}");
        NetworkAdapter = new LiteNetServerAdapter();
        NetworkAdapter.Start();
        NetworkAdapter.ConnectedEvent += Connection;
        
    }
    public override void _Ready()
    {
        World = GetNode<WorldNode>("World");
        World.LoadState(GameState);


        GameState.CharacterAddedEvent += World.AddCharacter;
        GameState.CharacterAddedEvent += BroadcastCharacter;
    }
    private void BroadcastCharacter(Character character) 
        => NetworkAdapter.Send(new CreateDatagram(ObjectToDTO.Convert(character)));


    private void Connection(INetworkConnectionAdapter connection)
    {
        PlayerNetworkHandler player = new(connection, GameState);
        _players.Add(player);

        foreach (ChunkRecord chunkRecord in GameState.Chunks.GetChunks()) player.Adapter.Send(CommonToDTO.Convert(chunkRecord));
        foreach (var character in GameState.Characters.Values) player.Adapter.Send(ObjectToDTO.Convert(character));
        player.Adapter.Send(new SetPlayerCommandDTO(player.Character.ID));
        
    }




    public override void _Process(double delta)
    {
        NetworkAdapter.Poll();
        // outgoing streaming
        foreach(var player in GameState.Characters.Values)
        {
            NetworkAdapter.Send(player.GetUpdate(CharacterAttribute.Position));
            foreach(AbstractUpdateDTO update in player.PollUpdates()) NetworkAdapter.Send(update);
            
        }

        // outgoing messages
        foreach (AbstractCommandDTO command in GameState.PollEvents()) NetworkAdapter.Send(command);
    }
}
