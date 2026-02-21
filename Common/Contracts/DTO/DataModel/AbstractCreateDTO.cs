using MessagePack;

namespace OpenTrenches.Common.Contracts.DTO.DataModel;


[MessagePackObject]
[Union(0,   typeof(CharacterDTO))]
[Union(10,  typeof(WorldChunkDTO))]
[Union(50,  typeof(TeamDTO))]
public abstract record class AbstractCreateDTO {}