using System.Numerics;
using Raylib_cs;

namespace GravSim;

public class CameraHandler2D(Vector2d screenSize) : CameraHandler
{
    private float targetZoom = 1f;
    private Vector3d targetPosition;
    // private Vector3d _targetTarget;
    private Camera2D camera = new()
    {
        Target = new Vector2(0, 0),
        Offset = screenSize / 2f,
        Zoom = 1.0f,
        Rotation = 0.0f,
    };
    private Vector2d screenSize = screenSize;

    public override void MoveTo(Vector3d pos)
    {
        // _camera.Offset = pos.XY;
        // _camera.Offset = new Vector2(0, 0);
        targetPosition = pos.XY;
        // _camera.Offset = pos.XY;
    }


    public override float GetZoom()
    {
        return camera.Zoom;
    }

    public override Vector3d GetWorldToScreen(Vector3d pos)
    {
        return Raylib.GetWorldToScreen2D(pos.XY, camera);
        // throw new NotImplementedException();
        // return _camera.Target - pos + _camera.Offset;
    }

    public override void PanInput(Vector2d delta)
    {
        // side-to-side (abscissa): turn
        camera.Rotation += (float)(delta.X * PanSensitivity);
    }

    public override void ScrollInput(float delta)
    {
        targetZoom *= delta + 1;
    }

    public override void BeginMode()
    {
        Raylib.BeginMode2D(camera);
    }

    public override void EndMode()
    {
        Raylib.EndMode2D();
    }

    public override string ToString()
    {
        return $"Target: {camera.Target}\nOffset: {camera.Offset}\nZoom: {camera.Zoom:f2}\nRotation: {camera.Rotation:f2}\nMovement Interpolation Factor: {MovementInterpolationFactor}\nSnap: {Snap}";
    }

    public override Vector3d GetPosition()
    {
        return camera.Target;
    }

    public override void AnimationStep(float deltaTime)
    {
        camera.Target = Physics.CubicInterpolate(camera.Target, targetPosition, (float)MovementInterpolationFactor).XY;
        camera.Zoom = (float)Physics.CubicInterpolate(camera.Zoom, targetZoom, (float)MovementInterpolationFactor);
    }
}