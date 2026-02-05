using System;
using OpenTrenches.Common.Scene.World;
using OpenTrenches.Common.World;

namespace OpenTrenches.Server.Scene.World;

public partial class ServerChunkNode : BaseChunkNode
{
    public ServerChunkNode(IChunk Chunk) : base(Chunk)
    {
        CollisionLayer = SceneDefines.Map.TerrainLayer;
        CollisionMask = SceneDefines.Map.NilLayer;
    }
}