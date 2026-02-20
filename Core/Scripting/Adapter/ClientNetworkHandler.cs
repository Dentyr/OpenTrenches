
using OpenTrenches.Common.Multiplayer;
using OpenTrenches.Common.Contracts;
using OpenTrenches.Common.Contracts.DTO;
using System;
using System.Threading.Tasks;

namespace OpenTrenches.Core.Scripting.Adapter;

public class ClientNetworkHandler(INetworkConnectionAdapter Adapter) : AbstractNetworkHandler(Adapter)
{
    public ushort? PlayerID;

    public ClientState State = new();


    protected override void _DeserializeCreate(CreateDatagram datagram) => State?.Create(datagram.DTO);


    protected override void _DeserializeStream(StreamDatagram datagram)
    {
        throw new NotImplementedException();
    }

    protected override void _DeserializeUpdate(UpdateDatagram datagram) => State?.Update(datagram.Update);

    protected override void _DeserializeMessage(CommandDatagram message) => State?.Receive(message.DTO);

    internal async Task<bool> WaitUntilConnect(int attempts = 10)
    {
        for (int i = 0; i < attempts; i ++)
        {
            if (Adapter.Active) return true;
            await Task.Delay(100);
        }
        return false;
    }
}