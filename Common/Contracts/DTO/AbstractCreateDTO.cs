using MessagePack;
using OpenTrenches.Common.Contracts.DTO.DataModel;

namespace OpenTrenches.Common.Contracts.DTO;


[MessagePackObject]
[Union(0,   typeof(CharacterDTO))]
[Union(10,  typeof(WorldChunkDTO))]
[Union(50,  typeof(TeamDTO))]
public abstract record class AbstractCreateDTO {}