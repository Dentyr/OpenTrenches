using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using LiteNetLib;
using OpenTrenches.Common.Contracts;
using OpenTrenches.Common.Contracts.DTO.Discovery;

namespace OpenTrenches.Common.Multiplayer;


/// <summary>
/// Seeks active servers from the master server
/// </summary>
public class ConnectionAgent
{
    private readonly NetManager NetManager;
    private readonly EventBasedNetListener Listener;

    private int _polling = 0;

    /// <summary>
    /// The list of servers sent by the master server. Set to null whenever polled.
    /// </summary>
    public event Action<ServerRecord[]>? ReceivedServersEvent;
    public ConnectionAgent()
    {
        Listener = new();
        NetManager = new(Listener)
        {
            UnconnectedMessagesEnabled = true,
        };
        NetManager.Start();
    

        Listener.NetworkReceiveUnconnectedEvent += HandleReceive;
    }

    private void HandleReceive(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
        // Only update events while currently polling for records
        if (_polling == 0) return;

        // Notify receival of server records if successfully deserialized
        if (Serialization.TryDeserialize(reader.GetRemainingBytes(), out SendServerListDTO serverList))
        {
            ReceivedServersEvent?.Invoke([.. NetFromDTO.Convert(serverList)]);
            _polling = 0;
        }
    }

    /// <summary>
    /// Sends requests to the master server for server records <paramref name="attempts"/> number of times
    /// </summary>
    /// <returns></returns>
    public async void PollRecords(int attempts = 10)
    {
        // Only allow one thread to poll at a time
        if (Interlocked.Exchange(ref _polling, 1) != 0) return;
    
        // clear existing events
        NetManager.PollEvents();
        _polling = 1;


        try
        {
            for (int i = 0; i < attempts && _polling == 1; i ++)
            {
                // NetManager.SendUnconnectedMessage([0,1,23], new IPEndPoint(IPAddress.Loopback, 4000));
                NetManager.SendUnconnectedMessage(Serialization.Serialize(new RequestServerListDTO()), NetworkDefines.GetMasterEndPoint());
                NetManager.PollEvents();
                await Task.Delay(200);
            }
        }
        finally
        {
            _polling = 0;
        }
    }
}
