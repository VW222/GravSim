using System.Numerics;
using Raylib_cs;

namespace GravSim;

public abstract class CameraHandler
{
    public bool Snap = false;
    public double MovementInterpolationFactor = 0.25;
    public double PanSensitivity = 1.0;
    
    public abstract void MoveTo(Vector3d pos);
    public abstract float GetZoom();
    public abstract Vector3d GetWorldToScreen(Vector3d pos);
    public abstract void PanInput(Vector2d delta);
    public abstract void ScrollInput(float delta);
    public abstract void BeginMode();
    public abstract void EndMode();
    public abstract Vector3d GetPosition();
    public abstract void AnimationStep(float deltaTime);
}