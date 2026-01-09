
using System;
using OpenTrenches.Scene.World;
using OpenTrenches.Scripting.Multiplayer;
using OpenTrenches.Scripting.Datastream;
using OpenTrenches.Scripting.Player;

public class ClientNetworkHandler(INetworkConnectionAdapter Adapter) : AbstractNetworkHandler(Adapter)
{
    public ushort? PlayerID;
    public event Action<ushort>? PlayerCharacterSetEvent;

    public GameState? State = new();


    protected override void _DeserializeCreate(CreateDatagram datagram)
    {
        // throw new Exception();
        State?.Create(datagram.TargetType, datagram.TargetId, datagram.Value);
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
                State?.Update(datagram.TargetType, datagram.TargetId, datagram.Update);
                break;
        }
    }

    protected override void _DeserializeMessage(MessageDatagram message)
    {
        switch (message.MessageCategory)
        {
            case MessageCategory.Setplayer:
                PlayerCharacterSetEvent?.Invoke(Serialization.Deserialize<ushort>(message.Item));
                break;
        }
    }
}