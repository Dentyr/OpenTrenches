
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using LiteNetLib;
using LiteNetLib.Utils;
using OpenTrenches.Scene;

namespace OpenTrenches.Scripting.Multiplayer;

public class LiteNetServerAdapter : IServerNetworkAdapter
{
    private NetManager Server { get; }
    private EventBasedNetListener Listener { get; }

    private Dictionary<NetPeer, LiteNetConnectionAdapter> _adapterDictionary = [];

    public event Action<INetworkConnectionAdapter>? ConnectedEvent;
    
    public LiteNetServerAdapter()
    {
        Listener = new();
        Server = new(Listener);


        Listener.ConnectionRequestEvent += HandleRequest;

        Listener.PeerConnectedEvent += HandleConnect;

        Listener.NetworkReceiveEvent += HandleReceive;

        // Listener.DeliveryEvent 

    }

    private void HandleReceive(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
    {
        _adapterDictionary[peer].HandleReceive(reader.GetRemainingBytes());
    }

    private void HandleConnect(NetPeer peer)
    {
        Console.WriteLine("We got connection: {0}", peer);  // Show peer IP
        NetDataWriter writer = new NetDataWriter();         // Create writer class
        writer.Put("Hello client!");                        // Put some string
        peer.Send(writer, DeliveryMethod.ReliableOrdered);  // Send with reliability

        LiteNetConnectionAdapter connection = new LiteNetConnectionAdapter(peer);
        _adapterDictionary.Add(peer, connection);
        ConnectedEvent?.Invoke(connection);
    }
    private void HandleRequest(ConnectionRequest request)
    {
        if(Server.ConnectedPeersCount < 10 /* max connections */)
            request.AcceptIfKey(NetworkDefines.Key);
        else
            request.Reject();
    }

    public void Poll()
    {
        Server.PollEvents();
    }

    public void Start()
    {
        Server.Start(NetworkDefines.ServerPort /* port */);
    }

    public void Stop() => Server.Stop();
}

public class LiteNetClientAdapter : IClientNetworkAdapter
{
    private EventBasedNetListener Listener { get; }
    private NetManager Client { get; }
    public LiteNetClientAdapter()
    {
        Listener = new();
        Client = new(Listener);

        Listener.NetworkReceiveEvent += HandleNetworkReceive;
    }
    private void HandleNetworkReceive(NetPeer peer, NetPacketReader dataReader, byte channel, DeliveryMethod deliveryMethod)
    {
        Console.WriteLine("We got: {0}", dataReader.GetString(100 /* max length of string */));
        dataReader.Recycle();
    }

    public event Action<INetworkConnectionAdapter>? ConnectedEvent;

    public INetworkConnectionAdapter Connect(string hostname)
    {
        return new LiteNetConnectionAdapter(Client.Connect("localhost", NetworkDefines.ServerPort, NetworkDefines.Key));
    }

    public void Poll()
    {
        Client.PollEvents();
    }

    public void Start() => Client.Start();

    public void Stop() => Client.Stop();
}