
using System;
using OpenTrenches.Scene.World;
using OpenTrenches.Scripting.Multiplayer;
using OpenTrenches.Scripting.Datastream;
using OpenTrenches.Scripting.Player;

public class ClientNetworkHandler(INetworkConnectionAdapter Adapter) : AbstractNetworkHandler(Adapter)
{
    public WorldNode? World;


    protected override void _DeserializeCreate(CreateDatagram datagram)
    {
        switch (datagram.TargetType)
        {
            case ObjectCategory.Character:
                World?.AddCharacter(Serialization.Deserialize<Character>(datagram.Value));
                break;
        }
    }


    protected override void _DeserializeStream(StreamDatagram datagram)
    {
        switch (datagram.StreamCategory)
        {
            case StreamCategory.Input:
                break;
        }
    }

    protected override void _DeserializeUpdate(UpdateDatagram datagram)
    {
        switch (datagram.TargetType)
        {
            case ObjectCategory.Character:
                World?.Character?.Update(datagram.Update);
                break;
        }
    }
}