
using OpenTrenches.Common.Multiplayer;
using OpenTrenches.Common.Contracts;

namespace OpenTrenches.Scripting;

public class ClientNetworkHandler(INetworkConnectionAdapter Adapter) : AbstractNetworkHandler(Adapter)
{
    public ushort? PlayerID;

    public ClientState? State = new();


    protected override void _DeserializeCreate(CreateDatagram datagram) => State?.Create(datagram.TargetType, datagram.TargetId, datagram.Value);


    protected override void _DeserializeStream(StreamDatagram datagram)
    {
        switch (datagram.StreamCategory)
        {
            default:
                break;
        }
    }

    protected override void _DeserializeUpdate(UpdateDatagram datagram) => State?.Update(datagram.TargetType, datagram.TargetId, datagram.Update);

    protected override void _DeserializeMessage(MessageDatagram message) => State?.Receive(message.MessageCategory, message.Item);
}