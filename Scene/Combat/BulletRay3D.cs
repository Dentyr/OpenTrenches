using System;
using Godot;

namespace OpenTrenches.Scene.Combat;

public partial class BulletRay3D : MeshInstance3D
{
    private readonly double _decayTarget;
    private double _decay = 0;
    private OmniLight3D _omni;
    public BulletRay3D(Vector3 begin, Vector3 end, double decayTarget = 0.2d)
    {
        begin += (end-begin).LimitLength(0.3f);
        Position = begin;
        _decayTarget = decayTarget;
        ImmediateMesh mesh = new();
        mesh.SurfaceBegin(Mesh.PrimitiveType.Lines);
        mesh.SurfaceAddVertex(Vector3.Zero);
        mesh.SurfaceAddVertex(end - begin);
        mesh.SurfaceEnd();
        // mesh.SurfaceSetColor(Colors.LightYellow);
        mesh.SurfaceSetMaterial(0, new StandardMaterial3D()
        {
            EmissionEnabled = true,
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            Emission = new Color(10, 10, 5, 1),
            // EmissionIntensity = 10,
        });
        Mesh = mesh;

        // bullet flash
        _omni = new();
        _omni.LightColor = Colors.LightYellow;
        AddChild(_omni);

    }

    public override void _Process(double delta)
    {
        _omni.LightEnergy = (float)(1 - Math.Pow(_decay/_decayTarget, 5))*1f;
        _decay += delta;
        if (_decay > _decayTarget)
        {
            QueueFree();
        }
        
    }
}