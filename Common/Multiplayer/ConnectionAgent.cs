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
/// Seeks
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

    // void IDisposable.Dispose() => NetManager.Stop();
}

// public class ConnectionServer : IDisposable
// {
//     private readonly NetManager NetManager;
//     private readonly EventBasedNetListener Listener;
    
//     private List<ServerRecord> _records = [];

//     public ConnectionServer()
//     {
//         Listener = new();
//         NetManager = new(Listener);
//         NetManager.Start();

//         Listener.NetworkReceiveUnconnectedEvent += HandleReceive;
//     }

//     /// <summary>
//     /// Adds <paramref name="record"/> if unique endpoint
//     /// </summary>
//     public void AddRecord(ServerRecord record) {
//         if (!_records.Any(stored => stored.EndPoint == record.EndPoint)) _records.Add(record);
//     }

//     /// <summary>
//     /// Remove any instances of <paramref name="record"/>'s endpoint
//     /// </summary>
//     public void RemoveRecord(ServerRecord record) {
//         _records.RemoveAll(stored => stored.EndPoint == record.EndPoint);
//     }

//     private void HandleReceive(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
//     {
//         if (Serialization.TryDeserialize(reader.GetRemainingBytes(), out RequestServerListDTO _))
//             NetManager.SendUnconnectedMessage(Serialization.Serialize(NetToDTO.Convert(_records)), remoteEndPoint);
//     }


//     public void Poll() => NetManager.PollEvents();

//     void IDisposable.Dispose() => NetManager.Stop();
// }