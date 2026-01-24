using System;
using System.Net;
using LiteNetLib;
using OpenTrenches.Common.Contracts;

namespace OpenTrenches.Common.Multiplayer;

public class LiteNetConnectionAdapter : INetworkConnectionAdapter
{
    private NetPeer Peer { get; }

    public IPAddress Address => Peer.Address;
    public ushort Port => (ushort)Peer.Port;

    public bool Active => Peer.ConnectionState == ConnectionState.Connected;

    public event Action<ReadOnlyMemory<byte>>? ReceiveEvent;

    public LiteNetConnectionAdapter(NetPeer Peer)
    {
        this.Peer = Peer;
    }


    public void HandleReceive(byte[] receive)
    {
        ReceiveEvent?.Invoke(receive.AsMemory());
    }

    /// <summary>
    /// Sequenced channel delivery
    /// </summary>
    public void Stream(byte[] payload) => Peer.Send(payload, DeliveryMethod.Sequenced);

    /// <summary>
    /// reliable ordered channel delivery
    /// </summary>
    public void Message(byte[] payload) => Peer.Send(payload, DeliveryMethod.ReliableOrdered);

    public void Send(Datagram datagram)
    {
        if (datagram.Streamed) Stream(Serialization.Serialize<Datagram>(datagram));
        else Message(Serialization.Serialize<Datagram>(datagram));
    }
}