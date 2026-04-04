using System;
using Godot;
using OpenTrenches.Common.Contracts.Defines;

namespace OpenTrenches.Core.Scene.Combat;

public partial class BulletRay2D : Line2D
{
    private readonly double _decayTarget;
    private double _decay = 0;
    public BulletRay2D(Vector2 begin, Vector2 end, double decayTarget = 0.05d)
    {
        begin += (end-begin).LimitLength(0.3f * CommonDefines.CellSize);
        begin *= CommonDefines.CellSize;
        end *= CommonDefines.CellSize;
        Points = [begin, end];
        _decayTarget = decayTarget;

    }

    public override void _Process(double delta)
    {
        _decay += delta;
        if (_decay > _decayTarget)
        {
            QueueFree();
        }
        
    }
}