using MessagePack;
using OpenTrenches.Scripting.Datastream;

namespace OpenTrenches.Scripting.Multiplayer;

[MessagePackObject]
[Union(0, typeof(CreateDatagram))]
[Union(1, typeof(UpdateDatagram))]
[Union(2, typeof(StreamDatagram))]
[Union(3, typeof(MessageDatagram))]
public abstract class Datagram() {}

/// <summary>
/// Datagram for generic streamed data
/// </summary>
[MessagePackObject]
public class StreamDatagram(StreamCategory StreamCategory, byte[] Item) : Datagram
{
    [Key(0)]
    public StreamCategory StreamCategory { get; } = StreamCategory;
    [Key(1)]
    public byte[] Item { get; } = Item;
}


/// <summary>
/// Datagram for generic messages
/// </summary>
[MessagePackObject]
public class MessageDatagram(MessageCategory MessageCategory, byte[] Item) : Datagram
{
    [Key(0)]
    public MessageCategory MessageCategory { get; } = MessageCategory;
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