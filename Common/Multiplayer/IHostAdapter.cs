
using System;
using OpenTrenches.Common.Contracts;
using OpenTrenches.Common.Contracts.DTO;

namespace OpenTrenches.Common.Multiplayer;

/// <summary>
/// Represents a host, and the necessary functions for <see cref="OpenTrenches"/> to connect with other hosts.
/// </summary>
public interface IHostNetworkAdapter
{

    /// <summary>
    /// Manually checks for new incoming events
    /// </summary>
    public void Poll();

    /// <summary>
    /// Enables services and listening
    /// </summary>
    public void Start();

    /// <summary>
    /// Disables services;
    /// </summary>
    void Stop();
}

public interface IClientNetworkAdapter : IHostNetworkAdapter
{
    /// <summary>
    /// Starts an asynchronous connection attempt to hostname
    /// </summary>
    /// <param name="hostname"></param>
    public INetworkConnectionAdapter Connect(string hostname);

}

public interface IServerNetworkAdapter : IHostNetworkAdapter
{
    public event Action<INetworkConnectionAdapter> ConnectedEvent;

    public void Send(Datagram datagram);
    
    public void Send(AbstractUpdateDTO dTO) =>  Send(new UpdateDatagram(dTO));
    public void Send(AbstractDTO dTO) =>  Send(new CreateDatagram(dTO));
    public void Send(AbstractCommandDTO dTO) =>  Send(new CommandDatagram(dTO));
    public void Send(AbstractStreamDTO dTO) =>  Send(new StreamDatagram(dTO));
}