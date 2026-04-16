using Godot;
using OpenTrenches.Common.Multiplayer;
using System;

public partial class MainMenuLayer : CanvasLayer
{
    private PagesNode _pages = null!;

    private ServersPage _servers = null!;

    private TitlePage _title = null!;

    public event Action<ServerRecord>? TryJoinServerEvent;

    public override void _Ready()
    {
        _pages = GetNode<PagesNode>("Pages");
        _pages.HideAll();

        _title = _pages.GetPage<TitlePage>();
        _title.NavigateServersEvent += OpenServers;

        _servers = _pages.GetPage<ServersPage>();
        _servers.NavigateTitleEvent += OpenTitle;
        _servers.ClickServerEvent += record => TryJoinServerEvent?.Invoke(record);


        _pages.Raise<TitlePage>();
    }

    public void Initialize(ConnectionAgent agent)
    {
        _servers.Initialize(agent);
    }

    public void OpenTitle()
    {
        _pages.Raise<TitlePage>();
    }

    public void OpenServers()
    {
        _pages.Raise<ServersPage>();
    }
}
