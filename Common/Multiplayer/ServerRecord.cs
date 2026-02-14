using System.Net;
using MessagePack;

namespace OpenTrenches.Common.Multiplayer;

public record class ServerRecord(
    IPEndPoint EndPoint
) {}