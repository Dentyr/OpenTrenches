using System;
using System.Buffers;
using System.Net;
using System.Runtime.InteropServices;
using LiteNetLib;

namespace OpenTrenches.Scripting.Multiplayer;

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

    public void Stream(byte[] payload) => Peer.Send(payload, DeliveryMethod.Sequenced);

    public void Message(byte[] payload) => Peer.Send(payload, DeliveryMethod.ReliableOrdered);
}