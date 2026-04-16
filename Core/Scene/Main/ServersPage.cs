using Godot;
using OpenTrenches.Common.Multiplayer;
using System;
using System.Collections.Generic;
using System.Net;

public partial class ServersPage : Control
{
    private ConnectionAgent _agent = null!;


    private Button _refreshButton = null!;
    private Button _mainmenuButton = null!;

    // private Dictionary<string, Button> items;
    private Control _servers = null!;

    //*

    public event Action? NavigateTitleEvent;

    public event Action<ServerRecord>? ClickServerEvent;



    public override void _Ready()
    {
        _refreshButton = GetNode<Button>("Refresh");
        _refreshButton.Pressed += RequestRefresh;

        _mainmenuButton = GetNode<Button>("Button");
        _mainmenuButton.Pressed += () => NavigateTitleEvent?.Invoke();

        _servers = GetNode<Control>("ServersScroll/Content");
    }

    private void RequestRefresh()
    {
        _agent.PollRecords();
    }

    public void Initialize(ConnectionAgent agent)
    {
        _agent = agent;
        agent.ReceivedServersEvent += RefreshList;
    }

    private void RefreshList(ServerRecord[] servers)
    {
        foreach (var child in _servers.GetChildren()) if (child is CanvasItem item) item.QueueFree();
        foreach (ServerRecord server in servers)
        {
            var node = new ServerButton(server);
            node.Pressed += () => ClickServerEvent?.Invoke(server);
            _servers.AddChild(node);
        }
    }
}
