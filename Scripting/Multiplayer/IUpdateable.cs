using MessagePack;

namespace OpenTrenches.Scripting.Multiplayer;

public interface IUpdateable<UpdateType>
{
    public void Update(Update update);
    public Update GetUpdate(UpdateType type);
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