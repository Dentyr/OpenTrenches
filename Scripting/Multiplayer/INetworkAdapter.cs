using System;
using System.Net;
using System.Threading.Tasks;
using LiteNetLib;
using OpenTrenches.Scripting.Datastream;

namespace OpenTrenches.Scripting.Multiplayer;

/// <summary>
/// Represents an alive connection to an endpoint, and the necessary functions for <see cref="OpenTrenches"/>
/// </summary>
public interface INetworkConnectionAdapter
{
    public IPAddress Address { get; }
    public ushort Port { get; }

    public bool Active { get; }
    
    public event Action<ReadOnlyMemory<byte>>? ReceiveEvent;

    public void Send(Datagram datagram);
}