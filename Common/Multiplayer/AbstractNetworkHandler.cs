using System;
using OpenTrenches.Common.Contracts;

namespace OpenTrenches.Common.Multiplayer;

public abstract class AbstractNetworkHandler
{
    public INetworkConnectionAdapter Adapter { get; }

    protected AbstractNetworkHandler(INetworkConnectionAdapter Adapter)
    {
        this.Adapter = Adapter;
        Adapter.ReceiveEvent += HandleInput;
    }

    public void HandleInput(ReadOnlyMemory<byte> packet)
    {
        var datagram = Serialization.Deserialize<Datagram>(packet);
        if (datagram is CreateDatagram create) _DeserializeCreate(create);
        else if (datagram is StreamDatagram stream) _DeserializeStream(stream);
        else if (datagram is UpdateDatagram update) _DeserializeUpdate(update);
        else if (datagram is CommandDatagram message) _DeserializeMessage(message);
    }
    protected abstract void _DeserializeCreate(CreateDatagram create);
    protected abstract void _DeserializeStream(StreamDatagram stream);
    protected abstract void _DeserializeUpdate(UpdateDatagram update);
    protected abstract void _DeserializeMessage(CommandDatagram message);
}