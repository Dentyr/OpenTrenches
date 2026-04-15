using System;
using System.Net;
using OpenTrenches.Common.Contracts;
using OpenTrenches.Common.Contracts.DTO;
using OpenTrenches.Common.Contracts.DTO.DataModel;
using OpenTrenches.Common.Contracts.DTO.PlayerCommands;
using OpenTrenches.Common.Contracts.DTO.UpdateModel;
using OpenTrenches.Common.Multiplayer;
using OpenTrenches.Core.Scripting.Adapter;

namespace OpenTrenches.Core.Scripting;

public class ClientNetworkManager
{
    //* Networking
    private IClientNetworkAdapter NetworkAdapter { get; }
    private ClientNetworkHandler? ClientNetworkHandler;

    public bool CanSend => ClientNetworkHandler is not null;

    private readonly ConnectionAgent ConnectionAgent;

    public event Action<ClientState>? JoinGameEvent;

    public ClientNetworkManager()
    {
        //* Network

        NetworkAdapter = new LiteNetClientAdapter();
        NetworkAdapter.Start();

        ConnectionAgent = new();
        ConnectionAgent.ReceivedServersEvent += UpdateServerList;
    }

    public void Send(AbstractUpdateDTO dTO) =>  ClientNetworkHandler?.Adapter.Send(new UpdateDatagram(dTO));
    public void Send(AbstractCreateDTO dTO) =>  ClientNetworkHandler?.Adapter.Send(new CreateDatagram(dTO));
    public void Send(AbstractCommandDTO dTO) =>  ClientNetworkHandler?.Adapter.Send(new CommandDatagram(dTO));
    public void Send(AbstractStreamDTO dTO) =>  ClientNetworkHandler?.Adapter.Send(new StreamDatagram(dTO));

    
    /// <summary>
    /// Informs the user that they can connect to <paramref name="servers"/>
    /// </summary>
    /// <param name="obj"></param>
    private void UpdateServerList(ServerRecord[] servers)
    {
        //TODO make available to player
        if (servers.Length > 0) TryJoin(servers[0].EndPoint);
    }
    /// <summary>
    /// Attempts to connect to the game server at <paramref name="endPoint"/>
    /// </summary>
    private async void TryJoin(IPEndPoint endPoint)
    {
        //TODO consider failure to game instance
        //TODO clarify order of network and load events
        lock (this)
        {
            ClientNetworkHandler = new(NetworkAdapter.Connect(endPoint));
        }
        JoinGameEvent?.Invoke(ClientNetworkHandler.State);
    }
    public void PollAvailableServers()
    {
        ConnectionAgent.PollRecords();
    }

    public void Poll()
    {
        NetworkAdapter.Poll();
    }
}