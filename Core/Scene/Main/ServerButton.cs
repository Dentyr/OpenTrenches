using Godot;
using OpenTrenches.Common.Multiplayer;
using System;

public partial class ServerButton : Button
{
    public ServerButton(ServerRecord server)
    {
        Text = server.EndPoint.ToString();
    }
}
