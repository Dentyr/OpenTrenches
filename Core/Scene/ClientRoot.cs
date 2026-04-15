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
    ClientNetworkManager ClientNetworkManager;
    
    ClientGameRoot ClientGameRoot = null!;



    public ClientRoot()
    {
        ClientNetworkManager = new();
        ClientNetworkManager.JoinGameEvent += SetState;


    }

    private void SetState(ClientState state)
    {
        ClientGameRoot?.SetState(state);
    }




    //* Initialize godot items
    public override void _Ready()
    {
        ClientGameRoot = GetNode<ClientGameRoot>("GameRoot");
        ClientGameRoot.OutgoingUpdateEvent += ClientNetworkManager.Send;
        ClientGameRoot.OutgoingCreateEvent += ClientNetworkManager.Send;
        ClientGameRoot.OutgoingCommandEvent += ClientNetworkManager.Send;
        ClientGameRoot.OutgoingStreamEvent += ClientNetworkManager.Send;
        //* Try to connect to server
        ClientNetworkManager.PollAvailableServers();
    }







    public override void _Process(double delta)
    {
        ClientNetworkManager.Poll();
    }
}