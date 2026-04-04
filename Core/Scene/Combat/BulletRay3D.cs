using System;
using Godot;
using OpenTrenches.Common.Contracts.Defines;

namespace OpenTrenches.Core.Scene.Combat;

public partial class BulletRay3D : Line2D
{
    private readonly double _decayTarget;
    private double _decay = 0;
    public BulletRay3D(Vector3 begin, Vector3 end, double decayTarget = 0.05d)
    {
        begin += (end-begin).LimitLength(0.3f * CommonDefines.CellSize);
        begin *= CommonDefines.CellSize;
        end *= CommonDefines.CellSize;
        this.Points = [new(begin.X, begin.Z), new(end.X, end.Z)];
        Console.WriteLine("in " + this.Points.Stringify());
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