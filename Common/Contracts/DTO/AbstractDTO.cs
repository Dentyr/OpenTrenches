using MessagePack;

namespace OpenTrenches.Common.Contracts.DTO;


[MessagePackObject]
[Union(0, typeof(CharacterDTO))]
[Union(10, typeof(WorldChunkDTO))]
public abstract record class AbstractDTO {}