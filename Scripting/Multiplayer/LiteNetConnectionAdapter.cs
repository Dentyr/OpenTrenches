using System;
using System.Net;
using LiteNetLib;

namespace OpenTrenches.Scripting.Multiplayer;

public class LiteNetConnectionAdapter : INetworkConnectionAdapter
{
    private NetPeer Peer { get; }

    public IPAddress Address => Peer.Address;
    public ushort Port => (ushort)Peer.Port;

    public bool Active => Peer.ConnectionState == ConnectionState.Connected;

    public event Action<byte[]>? ReceiveEvent;
    public event Action? TerminatedEvent;

    public LiteNetConnectionAdapter(NetPeer Peer)
    {
        this.Peer = Peer;
    }

    public void Stream(byte[] datagram)
    {
        Peer.Send(datagram, DeliveryMethod.Sequenced);
    }

    public void HandleReceive(byte[] receive)
    {
        ReceiveEvent?.Invoke(receive);
    }
}