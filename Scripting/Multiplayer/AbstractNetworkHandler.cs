using System;
using System.Numerics;
using OpenTrenches.Scripting.Datastream;

namespace OpenTrenches.Scripting.Multiplayer;

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
        else if (datagram is MessageDatagram message) _DeserializeMessage(message);
    }
    protected abstract void _DeserializeCreate(CreateDatagram create);
    protected abstract void _DeserializeStream(StreamDatagram stream);
    protected abstract void _DeserializeUpdate(UpdateDatagram update);
    protected abstract void _DeserializeMessage(MessageDatagram message);
}