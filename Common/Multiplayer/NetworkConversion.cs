using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using OpenTrenches.Common.Contracts.DTO.Discovery;

namespace OpenTrenches.Common.Multiplayer;

public class NetToDTO
{
    public static SendServerListDTO Convert(IEnumerable<ServerRecord> records) =>
        new([.. records.Select(rec => new ServerEndPointDTO(rec.EndPoint.Address.GetAddressBytes(), (ushort)rec.EndPoint.Port))]);
}
public class NetFromDTO
{
    public static IEnumerable<ServerRecord> Convert(SendServerListDTO serverList) 
        => serverList.ServerList.Select(dto => new ServerRecord(new IPEndPoint(new IPAddress(dto.Address), dto.Port)));
}