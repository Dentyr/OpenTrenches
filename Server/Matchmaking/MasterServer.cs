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
using OpenTrenches.Server.Matchmaking;

namespace OpenTrenches.Matchmaking;

//TODO consider failure to connect to master server
[GlobalClass]
/// <summary>
/// A node that maintains records of currently active server processes.
/// </summary>
public partial class MasterServer : Node
{
    /// <summary>
    /// Maps PIDs to their servers
    /// </summary>
    private readonly Dictionary<int, ServerProcessRecord> _servers = [];

    private ProcessStartInfo GetProcessStartInfo(int port)
    {
        string[] args = OS.GetCmdlineArgs();
        string gdArgs = $"Server/Scene/GameRoot.tscn --headless --port {port}";
        for(int i = 0; i < args.Length; i ++)
        {
            if (args[i] == "--debug-view")
            {
                gdArgs = $"Server/Scene/GameRoot.tscn --debug-collisions --port {port}";
            }
        }
        return new()
        {
            FileName = "godot-mono",
            Arguments = gdArgs,
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


    /// <summary>
    /// Returns a list of active servers
    /// </summary>
    private IEnumerable<ServerRecord> GetServerRecords()
    {
        lock(_servers)
        {
            return [.. _servers.Values
                .Where(server => server.Active)
                .Select(server => new ServerRecord(server.EndPoint))];
        }
    }

    private void HandleReceive(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
        if (Serialization.TryDeserialize(reader.GetRemainingBytes(), out RequestServerListDTO request)) {
            _netManager.SendUnconnectedMessage(Serialization.Serialize(NetToDTO.Convert(GetServerRecords())), remoteEndPoint);
        }
    }

    private ServerProcessRecord NewServer(int port)
    {
        var process = new Process
        {
            StartInfo = GetProcessStartInfo(port),
            EnableRaisingEvents = true
        };

        process.OutputDataReceived += (_, output) =>
        {
            if (output.Data != null) 
            {
                switch(output.Data)
                {
                    // When match is ended, stop advertising server as available
                    case Lifecycle.MatchEnd:
                        Console.WriteLine($"match ended for port {port}");
                        SetActivity(process.Id, false);
                        break;
                    default:
                        Console.WriteLine($"[Godot {port}] {output.Data}");
                        break;
                }
            }
        };

        process.ErrorDataReceived += (_, output) =>
        {
            if (output.Data != null) Console.WriteLine($"[Godot ERR {port}] {output.Data}");
        };

        process.Exited += (_, args) => HandleServerExited(process.Id);

        process.Start();
        int pid = process.Id;

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();


        ServerProcessRecord serverRecord = new(new IPEndPoint(IPAddress.Loopback, port), process);
        lock (_servers)
        {
            if (_servers.ContainsKey(pid))
            {
                if (!_servers[pid].Process.HasExited) throw new Exception("Duplicate keys found");

            }
            _servers[pid] = serverRecord;
        }
        return serverRecord;
    }
    private void SetActivity(int pid, bool active)
    {
        lock (_servers)
        {
            if (_servers.TryGetValue(pid, out var value))
                value.Active = active;
        }
    }
    /// <summary>
    /// Removes the process from the server list and recycles the port
    /// </summary>
    private void HandleServerExited(int pid)
    {
        lock (_servers)
        {
            if (_servers.Remove(pid, out ServerProcessRecord? record))
            {
                NewServer(record.EndPoint.Port);
            }
        }
    }

    public override void _Ready()
    {
        //TODO fix magic number document somewhere
        NewServer(3030);

        Console.CancelKeyPress += (_, _) => Shutdown();
        AppDomain.CurrentDomain.ProcessExit += (_, _) => Shutdown();

    }

    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest) 
            Shutdown();
    }

    private void Shutdown()
    {
        lock (_servers)
        {
            _netManager.Stop();
            foreach (Process process in 
                _servers.Values
                .Select(record => record.Process)
                .ToArray()
            )
            {
                try
                {
                    if (!process.HasExited)
                    {
                        process.Kill(entireProcessTree: true);
                    }
                }
                catch { }
            }
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
)
{
    /// <summary>
    /// Whether or not the server is still accepting new connections
    /// </summary>
    public bool Active { get; set; } = true;
}


public record class ServerInfo(
    IPEndPoint EndPoint,
    string Name
) {}