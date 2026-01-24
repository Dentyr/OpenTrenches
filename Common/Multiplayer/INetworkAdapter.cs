using System;
using System.Net;
using OpenTrenches.Common.Contracts;
using OpenTrenches.Common.Contracts.DTO;

namespace OpenTrenches.Common.Multiplayer;

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

    public void Send(AbstractUpdateDTO dTO) =>  Send(new UpdateDatagram(dTO));
    public void Send(AbstractDTO dTO) =>  Send(new CreateDatagram(dTO));
    public void Send(AbstractCommandDTO dTO) =>  Send(new CommandDatagram(dTO));
    public void Send(AbstractStreamDTO dTO) =>  Send(new StreamDatagram(dTO));
}