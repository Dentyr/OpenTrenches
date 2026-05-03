using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using OpenTrenches.Common.Collections;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.World;

namespace OpenTrenches.Server.Scene.World;

/// <summary>
/// Chunks are a store of tiles and structures in a localized area
/// </summary>
public class ServerChunk : AbstractChunk
{

    private HashSet<int> _structures = [];
    public IReadOnlySet<int> Structures => _structures;


    public ServerChunk() : base()
    {}

    public void AddStructureId(int structureId)
    {
        _structures.Add(structureId);
    }
    public void RemoveStructureId(int structureId)
    {
        _structures.Remove(structureId);
    }
}
