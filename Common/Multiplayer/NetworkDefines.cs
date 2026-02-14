using System.Net;

namespace OpenTrenches.Common.Multiplayer;

public static class NetworkDefines
{
    public const ushort ServerPort = 1912;
    public const string Key = "#*DiEU0(#3sJl1_0d)23Sp1}[w3j?]";
    public const int PacketSize = 2000;

    public static IPEndPoint GetMasterEndPoint() => new IPEndPoint(Dns.GetHostAddresses("localhost")[0], ServerPort);
}