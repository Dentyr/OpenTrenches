using MessagePack;
using OpenTrenches.Common.Contracts.DTO;

namespace OpenTrenches.Common.Contracts;

[MessagePackObject]
[Union(0, typeof(CreateDatagram))]
[Union(1, typeof(UpdateDatagram))]
[Union(2, typeof(StreamDatagram))]
[Union(3, typeof(CommandDatagram))]
public abstract class Datagram()
{
    [IgnoreMember]
    public virtual bool Streamed => false;
}


/// <summary>
/// Datagram for generic streamed data
/// </summary>
[MessagePackObject]
public class StreamDatagram(AbstractStreamDTO DTO) : Datagram
{
    [Key(0)]
    public AbstractStreamDTO DTO { get; } = DTO;
}


/// <summary>
/// Datagram for generic messages
/// </summary>
[MessagePackObject]
public class CommandDatagram(AbstractCommandDTO DTO) : Datagram
{
    [Key(0)]
    public AbstractCommandDTO DTO { get; } = DTO;
}


[MessagePackObject]
public class CreateDatagram(AbstractCreateDTO Item) : Datagram
{
    [Key(0)]
    public AbstractCreateDTO DTO { get; } = Item;
}


[MessagePackObject]
public class UpdateDatagram(AbstractUpdateDTO Update) : Datagram
{
    [Key(0)]
    public AbstractUpdateDTO Update { get; } = Update;

    public override bool Streamed => Update.Streamed;
}