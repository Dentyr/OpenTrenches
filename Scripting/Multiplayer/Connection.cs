/*
    Initial 
*/

// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Net;
// using System.Net.Sockets;
// using System.Security.Cryptography.X509Certificates;
// using System.Threading;
// using System.Threading.Tasks;
// using Godot;

// namespace OpenTrenches.Scripting.Multiplayer;
// public class Connection : INetworkConnectionAdapter
// {

//     public IPEndPoint EndPoint { get; }

//     public event Action<byte[]>? SendRequest;

//     //* Interface
//     public event Action<byte[]>? ReceiveEvent;
//     public event Action? TerminatedEvent;

//     public Connection(IPEndPoint EndPoint)
//     {
//         this.EndPoint = EndPoint;
//     }

//     public void PushReceive(byte[] byes)
//     {
//         ReceiveEvent?.Invoke(byes);
//     }
//     //* interface

//     public void Stream(byte[] datagram)
//     {
//         Console.WriteLine($"Datagram {datagram.Stringify()}");
//         SendRequest?.Invoke(datagram);
//     }

// }

// public class SendSocket(Socket Socket)
// {
//     private Socket Socket { get; } = Socket;

//     public void Send(byte[] send, EndPoint endPoint)
//     {
//         Socket.SendTo(send, endPoint);
//     }
// }


// /// <summary>
// /// <see cref="IHostNetworkAdapter"/> implementation using raw <see cref="Socket"/>
// /// </summary>
// public class SocketAdapter : IHostNetworkAdapter
// {
//     public static SocketAdapter GetClientAdapter()
//     {
//         Socket sock = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
//         //make ephemeral ports immediately
//         sock.Bind(new IPEndPoint(IPAddress.Any, 0)); 
//         return new SocketAdapter(sock);
//     }
//     public static SocketAdapter GetServerAdapter()
//     {
//         Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
//         sock.Bind(new IPEndPoint(IPAddress.Any, NetworkDefines.ServerPort));
//         return new SocketAdapter(sock);
//     }


//     private Socket? _socket;

//     private readonly byte[] RecvBuf = new byte[NetworkDefines.PacketSize];

//     private Queue<byte[]> _packetQueue = [];

//     private List<Connection> _connections = [];
//     public event Action<INetworkConnectionAdapter>? ConnectedEvent;

//     public Queue<byte[]> FlushQueue()
//     {
//         Queue<byte[]> temp = _packetQueue;
//         lock (_packetQueue)
//         {
//             _packetQueue = new();
//         }
//         return temp;
//     }

//     public SocketAdapter(Socket socket)
//     {
//         _socket = socket;
//     }
//     public async Task<bool> TryConnect(IPEndPoint target)
//     {
//         Connection connection = new(target);
//         connection.SendRequest += Stream;
//         ConnectedEvent?.Invoke(connection);
//         // for (int i = 0; i < 10; i ++)
//         // {
//         //     await Task.Delay(100);
//         // }
//         return true;
//     }
//     public void Listen()
//     {
//         while (_socket is not null)
//         {
//             Console.WriteLine("listening");
//             // _socket.Poll(100, SelectMode.SelectRead);
//             EndPoint src = new IPEndPoint(IPAddress.Any, 0);

//             int written = _socket.ReceiveFrom(buffer: RecvBuf, remoteEP: ref src);
//             Console.WriteLine();
//             Console.WriteLine(written);
//             Console.WriteLine(RecvBuf.Stringify());
//             // Thread.Sleep(10);
//         }
//     }


//     public void Stream(byte[] payload)
//     {
//         _socket?.SendTo(payload, new IPEndPoint(IPAddress.Loopback, NetworkDefines.ServerPort));
//     }



//     /// <summary>
//     /// Sends a sync packet through UDP to <paramref name="remoteEP"/>
//     /// </summary>
//     private void UDPSend(byte[] packet, EndPoint remoteEP) => _socket?.SendTo(packet, remoteEP);
// }
