using Godot;
using OpenTrenches.Core.Scene.World;
using OpenTrenches.Common.Multiplayer;
using OpenTrenches.Core.Scripting;
using OpenTrenches.Core.Scripting.Adapter;
using OpenTrenches.Common.Contracts.DTO;
using OpenTrenches.Core.Scene.GUI;
using OpenTrenches.Core.Scene.Gui;
using System.Linq;
using System.Collections.Generic;
using System;
using OpenTrenches.Core.Scripting.Player;
using OpenTrenches.Common.Contracts.DTO.ServerComands;
using OpenTrenches.Common.Contracts.DTO.PlayerCommands;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using OpenTrenches.Server.Scene;

namespace OpenTrenches.Core.Scene;

[GlobalClass]
public partial class ClientRoot : Node
{
    private ClientNetworkManager ClientNetworkManager;
    
    private ClientGameRoot ClientGameRoot = null!;

    private MainMenuLayer MainMenuRoot = null!;

    public ClientRoot()
    {
        ClientNetworkManager = new();
        ClientNetworkManager.JoinGameEvent += JoinGame;


    }

    private void JoinGame(ClientState state)
    {
        MainMenuRoot.Hide();
        ClientGameRoot?.SetState(state);
    }
    private void ExitGame()
    {
        ClientNetworkManager?.Disconnect();
        NavigateMainMenu();
    }
    private void NavigateMainMenu()
    {
        MainMenuRoot.Show();
    }




    //* Initialize godot items
    public override void _Ready()
    {
        ClientGameRoot = GetNode<ClientGameRoot>("GameRoot");
        ClientGameRoot.OutgoingUpdateEvent += ClientNetworkManager.Send;
        ClientGameRoot.OutgoingCreateEvent += ClientNetworkManager.Send;
        ClientGameRoot.OutgoingCommandEvent += ClientNetworkManager.Send;
        ClientGameRoot.OutgoingStreamEvent += ClientNetworkManager.Send;

        ClientGameRoot.ExitGameEvent += ExitGame;


        MainMenuRoot = GetNode<MainMenuLayer>("MainMenu");
        MainMenuRoot.Initialize(ClientNetworkManager.ConnectionAgent);
        MainMenuRoot.TryJoinServerEvent += record => ClientNetworkManager.TryJoin(record.EndPoint);
    }







    public override void _Process(double delta)
    {
        ClientNetworkManager.Poll();
    }
}