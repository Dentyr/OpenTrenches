using System;
using MessagePack;

namespace OpenTrenches.Scripting.Multiplayer;

public interface IUpdateable
{
    public void Update(Update update);
}

public class Update(byte Type, byte[] Payload)
{
    [Key(0)]
    public byte Type { get; } = Type;
    [Key(1)]
    public byte[] Payload { get; } = Payload;
}