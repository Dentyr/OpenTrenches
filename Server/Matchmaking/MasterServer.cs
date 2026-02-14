using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using LiteNetLib;
using OpenTrenches.Common.Contracts;
using OpenTrenches.Common.Contracts.DTO.Discovery;
using OpenTrenches.Common.Multiplayer;
using OpenTrenches.Core.Scene;

namespace OpenTrenches.Matchmaking;

[GlobalClass]
/// <summary>
/// A node that maintains records of currently active server processes.
/// </summary>
public partial class MasterServer : Node
{
    private readonly List<ServerProcessRecord> _servers = [];
    private ProcessStartInfo GetProcessStartInfo(ushort port)
    {
        return new()
        {
            FileName = "godot-mono",
            Arguments = $"Server/Scene/GameRoot.tscn --headless --port {port}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };
    }

    private readonly NetManager _netManager;
    private readonly EventBasedNetListener _listener;

    public MasterServer()
    {
        _listener = new();
        _netManager = new(_listener)
        {
            UnconnectedMessagesEnabled = true
        };

        _netManager.Start(NetworkDefines.ServerPort);
        _listener.NetworkReceiveUnconnectedEvent += HandleReceive;
    }


    private IEnumerable<ServerRecord> GetServerRecords()
        => _servers.Select(server => new ServerRecord(server.EndPoint));

    private void HandleReceive(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
        if (Serialization.TryDeserialize(reader.GetRemainingBytes(), out RequestServerListDTO request)) {
            _netManager.SendUnconnectedMessage(Serialization.Serialize(NetToDTO.Convert(GetServerRecords())), remoteEndPoint);
        }
    }

    private ServerProcessRecord NewServer(ushort port)
    {
        var process = new Process
        {
            StartInfo = GetProcessStartInfo(port),
            EnableRaisingEvents = true
        };
        process.OutputDataReceived += (_, output) =>
        {
            if (output.Data != null) Console.WriteLine($"[Godot {port}] {output.Data}");
        };

        process.ErrorDataReceived += (_, output) =>
        {
            if (output.Data != null) Console.WriteLine($"[Godot ERR {port}] {output.Data}");
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();


        ServerProcessRecord serverRecord = new(new IPEndPoint(IPAddress.Loopback, port), process);

        _servers.Add(serverRecord);
        return serverRecord;
    }
    public override void _Ready()
    {
        NewServer(3030);

        Console.CancelKeyPress += (_, e) => Shutdown();
        AppDomain.CurrentDomain.ProcessExit += (_, _) => Shutdown();

    }

    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest) Shutdown();
    }

    private void Shutdown()
    {
        _netManager.Stop();
        foreach (Process? process in _servers.Select(x => x.Process))
        {
            try
            {
                if (!process.HasExited)
                {
                    process.Kill(entireProcessTree: true);
                    process.WaitForExit(5000);
                }
            }
            catch { }
        }
    }

    public override void _Process(double delta)
    {
        _netManager.PollEvents();
    }
}

public record class ServerProcessRecord(
    IPEndPoint EndPoint,
    Process Process
) {}


public record class ServerInfo(
    IPEndPoint EndPoint,
    string Name
) {}