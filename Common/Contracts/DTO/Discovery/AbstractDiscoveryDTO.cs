using System.Net;
using MessagePack;

namespace OpenTrenches.Common.Contracts.DTO.Discovery;

[MessagePackObject]
[Union(0, typeof(SendServerListDTO))]
[Union(1, typeof(RequestServerListDTO))]
[Union(2, typeof(RegisterServerDTO))]
[Union(3, typeof(AckServerRegisteredDTO))]
public abstract record class AbstractDiscoveryDTO {}


[MessagePackObject]
public record class ServerEndPointDTO(
    [property: Key(0)] byte[] Address,
    [property: Key(1)] ushort Port
) {}
[MessagePackObject]
public record class SendServerListDTO (
    [property: Key(0)] ServerEndPointDTO[] ServerList
) : AbstractDiscoveryDTO {}

[MessagePackObject]
public record class RequestServerListDTO (
) : AbstractDiscoveryDTO {}


[MessagePackObject]
public record class RegisterServerDTO (
    [property: Key(0)] ServerEndPointDTO EndPoint
) : AbstractDiscoveryDTO {}


[MessagePackObject]
public record class AckServerRegisteredDTO (
    [property: Key(0)] bool Success,
    [property: Key(1)] ServerEndPointDTO? RegisteredServer
) : AbstractDiscoveryDTO {}