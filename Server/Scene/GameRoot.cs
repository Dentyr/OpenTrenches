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
using OpenTrenches.Server.Scripting.World;
using OpenTrenches.Server.Scripting.Teams;

namespace OpenTrenches.Server.Scene;

[GlobalClass]
public partial class GameRoot : Node
{
    public IServerNetworkAdapter NetworkAdapter;
    private WorldNode World { get; }

    private readonly List<PlayerNetworkHandler> _players = [];
    private readonly List<Character> _npcs = [];

    private ServerState GameState { get; }

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
        World.AddChild(new Camera2D() {Position = new Vector2(2500, 1500), Zoom=new Vector2(0.25f, 0.25f)});

        //* events
        // add character to world, notify clients, and ensure future updates go to clients.
        GameState.CharacterAddedEvent += HandleNewCharacter;

        // notify structures to clients.
        GameState.StructureCreatedEvent += HandleNewStructure;

        GameState.GameEndedEvent += HandleGameEnd;

        //* synchronize initial state
        foreach (Character character in GameState.Characters.Values) HandleNewCharacter(character);
        foreach (ServerStructure structure in GameState.Chunks.StructureDict.Values) HandleNewStructure(structure);

        //* Initialization
        for (int i = 0; i < 100; i ++)
        {
            var chara = GameState.CreateCharacter();
            _npcs.Add(chara);
            chara.NewAgent();
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

    private void HandleGameEnd(Team? victor)
    {
        NetworkAdapter.Send(new GameEndNotificationCommand(victor is null ? -1 : victor.ID));
    }
    #region handling game objects
    private void HandleNewCharacter(Character character)
    {
        World.AddCharacter(character);
        BroadcastCharacter(character);
        character.CharacterUpdateEvent += NetworkAdapter.Send;
    }
    private void HandleNewStructure(ServerStructure structure)
    {
        World.AddStructure(structure);
        BroadCastStructure(structure);
    }

    #endregion

    private void BroadcastCharacter(Character character) 
        => NetworkAdapter.Send(new CreateDatagram(ObjectToDTO.Convert(character)));
    private void BroadCastStructure(ServerStructure structure)
        => NetworkAdapter.Send(new CreateDatagram(ObjectToDTO.Convert(structure)));


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

        // outgoing messages
        foreach (AbstractCommandDTO command in GameState.PollEvents()) NetworkAdapter.Send(command);
    }
}
