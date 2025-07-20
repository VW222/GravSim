using Raylib_cs;

namespace GravSim;

public class CameraHandler3D : CameraHandler
{
    private float _targetFov; //camera.fovy
    private Vector3d _targetPosition; // camera.pos
    private Vector3d _targetTarget; // camera.target
    private Camera3D _camera;
    
    public float Fov
    {
        get => _camera.FovY;
        set => _camera.FovY = value;
    }
    
    public CameraHandler3D()
    {
        _camera = new Camera3D
        {
            Position = new Vector3d(100, 0, 0),
            Target = new Vector3d(0, 0, 0),
            Up = new Vector3d(0f, -1f, 0f),
            FovY = 60.0f,
            Projection = CameraProjection.Perspective,
        };
        _targetPosition = _camera.Position;
        _targetFov = _camera.FovY;
        _targetTarget = _camera.Target;
        Raylib.UpdateCamera(ref _camera, CameraMode.ThirdPerson);
    }

    public override void MoveTo(Vector3d pos)
    {
        throw new NotImplementedException();
    }

    public override float GetZoom()
    {
        throw new NotImplementedException();
    }
    
    public override Vector3d GetWorldToScreen(Vector3d pos)
    {
        return Raylib.GetWorldToScreen(pos, _camera);
    }

    public override void PanInput(Vector2d delta)
    {
        throw new NotImplementedException();
    }

    public override void ScrollInput(float delta)
    {
        throw new NotImplementedException();
    }

    public override void BeginMode()
    {
        throw new NotImplementedException();
    }

    public override void EndMode()
    {
        throw new NotImplementedException();
    }

    public override Vector3d GetPosition()
    {
        throw new NotImplementedException();
    }

    public override void AnimationStep(float deltaTime)
    {
        throw new NotImplementedException();
    }
}