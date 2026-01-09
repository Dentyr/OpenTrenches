using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using OpenTrenches.Scripting.Datastream;

namespace OpenTrenches.Scripting.Multiplayer;

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

    public void StreamBroadcast(Datagram datagram) => StreamBroadcast(Serialization.Serialize(datagram));
    public void MessageBroadcast(Datagram datagram) => MessageBroadcast(Serialization.Serialize(datagram));

    /// <summary>
    /// Sends <paramref name="datagram"/> to <see cref="Address"/> unreliably but in order.
    /// </summary>
    public void StreamBroadcast(byte[] datagram);
    /// <summary>
    /// Sends <paramref name="datagram"/> to <see cref="EndPoint"/> reliably and in order.
    /// </summary>
    public void MessageBroadcast(byte[] datagram);
}