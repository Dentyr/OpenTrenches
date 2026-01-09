using System;
using MessagePack;

namespace OpenTrenches.Scripting.Multiplayer;

public enum CommandType : byte
{
    Create,
    Stream,
    Update,
}