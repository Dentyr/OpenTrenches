using System;
using System.Collections.Generic;
using MessagePack;
using OpenTrenches.Scripting.Player;
using OpenTrenches.Scripting.Datastream;

namespace OpenTrenches.Scripting.Multiplayer;

public interface IUpdateable
{
    public void Update(Update update);
}

[MessagePackObject]
public class Update
{
    [SerializationConstructor]
    public Update(byte Type, byte[] Payload)
    {
        this.Type = Type;
        this.Payload = Payload;
    }

    [Key(0)]
    public byte Type { get; }
    [Key(1)]
    public byte[] Payload { get; }
}


[MessagePackObject]
[Union(0, typeof(CreateDatagram))]
[Union(1, typeof(UpdateDatagram))]
[Union(2, typeof(StreamDatagram))]
public abstract class Datagram() {}

[MessagePackObject]
public class StreamDatagram(StreamCategory StreamCategory, byte[] Item) : Datagram
{
    [Key(0)]
    public StreamCategory StreamCategory { get; } = StreamCategory;
    [Key(1)]
    public byte[] Item { get; } = Item;
}

[MessagePackObject]
public class CreateDatagram(ObjectCategory TargetType, ushort TargetId, byte[] Item) : Datagram
{
    [Key(0)]
    public ObjectCategory TargetType { get; } = TargetType;
    [Key(1)]
    public ushort TargetId { get; } = TargetId;
    [Key(2)]
    public byte[] Value { get; } = Item;
}


[MessagePackObject]
public class UpdateDatagram(ObjectCategory TargetType, ushort TargetId, Update Update) : Datagram
{
    [Key(0)]
    public ObjectCategory TargetType { get; } = TargetType;
    [Key(1)]
    public ushort TargetId = TargetId;
    [Key(2)]
    public Update Update { get; } = Update;
}