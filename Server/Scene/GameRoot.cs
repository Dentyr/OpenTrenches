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
using OpenTrenches.Server.Matchmaking;
using System.Threading.Tasks;
using System.Linq;

namespace OpenTrenches.Server.Scene;

[GlobalClass]
public partial class GameRoot : Node
{
    public IServerNetworkAdapter NetworkAdapter;
    private WorldNode World { get; }

    /// <summary>
    /// A dictionary of peer ID to player network handlers. 
    /// </summary>
    private readonly Dictionary<int, PlayerNetworkHandler> _players = [];

    private readonly AgentManager _npcManager = new();

    private ServerState GameState { get; }

    public GameRoot()
    {
        //* network setup
        Console.WriteLine($"PID {Process.GetCurrentProcess().ProcessName}");

        // 


        NetworkAdapter = new LiteNetServerAdapter();
        NetworkAdapter.ConnectedEvent += HandleConnect;
        NetworkAdapter.DisconnectedEvent += HandleDisconnect;
        
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
            _npcManager.AddCharacter(chara);
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

    public override void _ExitTree()
    {
        NetworkAdapter.Stop();
    }

    private void HandleGameEnd(Team? victor)
    {
        NetworkAdapter.Close();
        // Instruct clients to disconnect at earliest convenience. Wait for timeout or all exit
        NetworkAdapter.Send(new GameEndNotificationCommand(victor is null ? -1 : victor.ID));
        Console.WriteLine(Lifecycle.MatchEnd);
        // Drains incoming requests, and keeps polling to ensure clients received final message
        // keep polling until timeout or all players disconnected
        for (int i = 0; i < 50; i ++)
        {
            if (_players.Count == 0) 
                break;

            NetworkAdapter.Poll();
            Task.Delay(100).Wait();
        }
        NetworkAdapter.Stop();
        GetTree().Quit();
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


    private void HandleConnect(INetworkConnectionAdapter connection)
    {
        PlayerNetworkHandler player = new(connection, GameState);
        _players.Add(connection.Id, player);
        

        foreach (AbstractCreateDTO createDTO in GameState.GetInitDTOs()) player.Adapter.Send(createDTO);

        Character playerCharcter = player.Character;
        player.Adapter.Send(new SetPlayerCommandDTO(playerCharcter.ID, playerCharcter.PrimarySlot.AmmoLoaded, playerCharcter.PrimarySlot.AmmoStored, playerCharcter.Logistics));
        
        player.Adapter.Send(new InitializedNotificationCommand());
    }

    private void HandleDisconnect(INetworkConnectionAdapter adapter)
    {
        _players.Remove(adapter.Id);
    }




    public override void _Process(double delta)
    {
        NetworkAdapter.Poll();

        _npcManager.Process(World.CreateQueryService());
        //* player updates

        // outgoing messages
        foreach (AbstractCommandDTO command in GameState.PollEvents()) NetworkAdapter.Send(command);
    }
}
