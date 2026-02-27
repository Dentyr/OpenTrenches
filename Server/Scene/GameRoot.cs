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
using OpenTrenches.Common.Contracts.DTO.ServerComands;
using System.Threading;
using OpenTrenches.Common.Contracts.DTO.UpdateModel;
using OpenTrenches.Common.Contracts.DTO.DataModel;
using OpenTrenches.Server.Scripting.Player.Agent;

namespace OpenTrenches.Server.Scene;

[GlobalClass]
public partial class GameRoot : Node
{
    public IServerNetworkAdapter NetworkAdapter;
    private WorldNode World { get; }

    private readonly List<PlayerNetworkHandler> _players = [];
    private readonly List<AiCharacterController> _npcs = [];

    private ServerState GameState { get; } = new();

    public GameRoot()
    {
        //* network setup
        Console.WriteLine($"PID {Process.GetCurrentProcess().ProcessName}");

        // 


        NetworkAdapter = new LiteNetServerAdapter();
        NetworkAdapter.ConnectedEvent += Connection;
        
        //* model and adapter
        GameState = new();
        World = new(GameState);
        World.AddChild(new WorldEnvironment() {Environment = Core.Scene.SceneDefines.IlluminatedEnvironment});
        AddChild(World);

        //* debug camera
        World.AddChild(new Camera3D() {Position = new Vector3(0, 80, 0), Rotation = new Vector3((float)(-Math.PI / 2), 0f, 0f), Current = true});

        //* events
        GameState.CharacterAddedEvent += World.AddCharacter;
        GameState.CharacterAddedEvent += BroadcastCharacter;

        //* Initialization
        for (int i = 0; i < 100; i ++)
        {
            _npcs.Add(new AiCharacterController(GameState));
        }
    }
    public override void _EnterTree()
    {
        // get port number from command line arguments.
        string[] args = OS.GetCmdlineArgs();
        for(int i = 0; i < args.Length - 1; i ++)
        {
            if (args[i] == "--port" && int.TryParse(args[i + 1], out var portnum))
            {
                Console.WriteLine("Listening on port " + portnum);

                NetworkAdapter.Start(portnum);
                return;
            }
        }

        Console.Error.WriteLine("Failed to get port argument");
        GetTree().Quit();
        return;
    }

    private void BroadcastCharacter(Character character) 
        => NetworkAdapter.Send(new CreateDatagram(ObjectToDTO.Convert(character)));


    private void Connection(INetworkConnectionAdapter connection)
    {
        PlayerNetworkHandler player = new(connection, GameState);
        _players.Add(player);
        

        foreach (AbstractCreateDTO createDTO in GameState.GetInitDTOs()) player.Adapter.Send(createDTO);

        Character playerCharcter = player.Character;
        player.Adapter.Send(new SetPlayerCommandDTO(playerCharcter.ID, playerCharcter.PrimarySlot.AmmoLoaded, playerCharcter.PrimarySlot.AmmoStored, playerCharcter.Logistics));
        
        player.Adapter.Send(new InitializedNotificationCommand());
    }




    public override void _Process(double delta)
    {
        NetworkAdapter.Poll();

        //* player updates
        // outgoing updates
        foreach(Character character in GameState.Characters.Values)
        {
            NetworkAdapter.Send(character.GetUpdate(CharacterAttribute.Position));
            foreach(AbstractUpdateDTO update in character.PollUpdates()) NetworkAdapter.Send(update);
        }
        foreach(PlayerNetworkHandler player in _players)
        {
            foreach(AbstractUpdateDTO update in player.Character.PollPlayerUpdates()) player.Adapter.Send(update);
        }

        // outgoing messages
        foreach (AbstractCommandDTO command in GameState.PollEvents()) NetworkAdapter.Send(command);

        //* Npc updates
        foreach (var npc in _npcs)
        {
            npc.Think();
        }
    }
}
