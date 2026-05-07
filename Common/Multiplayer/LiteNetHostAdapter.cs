
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using LiteNetLib;
using LiteNetLib.Utils;
using OpenTrenches.Common.Contracts;
using OpenTrenches.Common.Contracts.DTO.ServerComands;

namespace OpenTrenches.Common.Multiplayer;

/// <summary>
/// Manages a server's connections and requests
/// </summary>
public class LiteNetServerAdapter : IServerNetworkAdapter
{
    /// <summary>
    /// True if adapter should be accepting connections
    /// </summary>
    private bool _open = true;

    private NetManager Server { get; }
    private EventBasedNetListener Listener { get; }

    private Dictionary<NetPeer, LiteNetConnectionAdapter> _adapterDictionary = [];

    public event Action<INetworkConnectionAdapter>? ConnectedEvent;
    public event Action<INetworkConnectionAdapter>? DisconnectedEvent;
    
    public LiteNetServerAdapter()
    {
        Listener = new();
        Server = new(Listener);


        Listener.ConnectionRequestEvent += HandleRequest;
        Listener.PeerConnectedEvent += HandleConnect;
        Listener.PeerDisconnectedEvent += HandleDisconnect;
        Listener.NetworkReceiveEvent += HandleReceive;
    }


    /// <summary>
    /// Directs an incoming packet to the adapter associated with <paramref name="peer"/>
    /// </summary>
    private void HandleReceive(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
    {
        _adapterDictionary[peer].HandleReceive(reader.GetRemainingBytes());
    }

    /// <summary>
    /// Handles an accepted connection request from <paramref name="peer"/>
    /// </summary>
    private void HandleConnect(NetPeer peer)
    {
        NetDataWriter writer = new();
        
        LiteNetConnectionAdapter connection = new(peer);
        _adapterDictionary.Add(peer, connection);
        ConnectedEvent?.Invoke(connection);
    }


    /// <summary>
    /// Handles <paramref name="peer"/> disconnecting
    /// </summary>
    private void HandleDisconnect(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        if (_adapterDictionary.Remove(peer, out LiteNetConnectionAdapter? adapter))
        {
            DisconnectedEvent?.Invoke(adapter);
        }
    }
    /// <summary>
    /// handles in incoming <paramref name="request"/>
    /// </summary>
    private void HandleRequest(ConnectionRequest request)
    {
        // Reject if past max connections or shutdown initited
        if (!_open || Server.ConnectedPeersCount >= 30)
            request.Reject();
        else
            request.AcceptIfKey(NetworkDefines.Key);
    }

    public void Poll() => Server.PollEvents();
    public void Start(int port) => Server.Start(port);
    public void Stop() => Server.Stop();

    /// <summary>
    /// Stop accepting connections
    /// </summary>
    public void Close() => _open = false;


    public void Send(Datagram datagram)
    {
        if (datagram.Streamed) foreach (var connecttion in _adapterDictionary.Values) connecttion.Stream(Serialization.Serialize<Datagram>(datagram));
        else foreach (var connecttion in _adapterDictionary.Values) connecttion.Message(Serialization.Serialize<Datagram>(datagram));
    }
}

/// <summary>
/// Manages connecting to and communication with a server
/// </summary>
public class LiteNetClientAdapter : IClientNetworkAdapter
{
    private EventBasedNetListener Listener { get; }
    private NetManager Client { get; }
    private LiteNetConnectionAdapter? Adapter { get; set; }

    public LiteNetClientAdapter()
    {
        Listener = new();
        Client = new(Listener);

        Listener.NetworkReceiveEvent += HandleNetworkReceive;
    }

    /// <summary>
    /// Directs an incoming packet to the adapter associated with <paramref name="peer"/>
    /// </summary>
    private void HandleNetworkReceive(NetPeer peer, NetPacketReader dataReader, byte channel, DeliveryMethod deliveryMethod)
    {
        Adapter?.HandleReceive(dataReader.GetRemainingBytes());
    }

    /// <summary>
    /// Attempts to connect to <paramref name="hostname"/>, returning a connection adapter to manage the connection.
    /// </summary>
    public INetworkConnectionAdapter Connect(string hostname)
    {
        Adapter = new LiteNetConnectionAdapter(Client.Connect("localhost", NetworkDefines.ServerPort, NetworkDefines.Key));
        return Adapter;
    }
    /// <summary>
    /// Attempts to connect to <paramref name="endpoint"/>, returning a connection adapter to manage the connection.
    /// </summary>
    public INetworkConnectionAdapter Connect(IPEndPoint endpoint)
    {
        Adapter = new LiteNetConnectionAdapter(Client.Connect(endpoint, NetworkDefines.Key));
        return Adapter;
    }


    public void Poll() => Client.PollEvents();
    public void Start() => Client.Start();
    public void Stop() => Client.Stop();
}