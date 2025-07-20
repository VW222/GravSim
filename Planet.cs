using System.Numerics;
using Raylib_cs;
namespace GravSim;

public class Planet
{
    public required Mass Mass;
    public string Name = "";
    public required Vector3d Position;
    public Vector3d Velocity = new(0, 0, 0);
    public float LabelMultiplier = 1.0f;
    public bool Immovable = false;
    
    private Vector3d worldCenter;
    private double worldScale;
    private CameraHandler cameraHandler;
    private float uiScale;
    
    public Color Color;
    public Distance Radius = Distance.FromSolarRadii(1);
    public int TrailLength = 0;
    private List<Vector3d> trail = [];

    private double apparentSize;

    public void Draw()
    {
        Raylib.DrawCircleV(cameraHandler.GetWorldToScreen(Position.XY * worldScale).XY, (float)apparentSize, Color);
        DrawLabel();
    }

    public void Update(Vector3d uworldCenter, double uworldScale,  CameraHandler ucameraHandler, float uuiScale)
    {
        this.worldCenter = uworldCenter;
        this.worldScale = uworldScale;
        cameraHandler = ucameraHandler;
        this.uiScale = uuiScale;
        apparentSize = ApparentSize();
    }

    private double ApparentSize()
    {
        // var d = Vector3.Distance(cameraHandler.GetPosition(), Position);
        // var theta = 2 * Math.Atan(Radius.GetMeters() / d);
        // return (theta / (_camera.FovY / 180f * Math.PI)) * (Math.Min(_screenSize.X, _screenSize.Y) / 2f);
        // return 100d;
        if (cameraHandler is CameraHandler2D)
        {
            return Math.Max(Radius.GetMeters() * worldScale * cameraHandler.GetZoom(), 2);
        }
        throw new NotImplementedException();
    }

    public void DrawBounds(Texture2D bounds)
    {
        // Console.WriteLine(this);
        float size = (float)Math.Max(apparentSize * 1.2, 15d) / bounds.Width * 2f;
        Raylib.DrawTextureEx(bounds, ScreenXY - new Vector2d(size, size) * bounds.Width / 2f, 0.0f, size, Color.White);
    }

    private void DrawLabel()
    {
        // float calcFontSize = (float)(20.0 * MathF.Sin(_camera.FovY / 180f * MathF.PI));
        float calcFontSize = 30f * uiScale;
        Vector2 size = Raylib.MeasureTextEx(Raylib.GetFontDefault(), Name, calcFontSize, uiScale * 4.5f);
        float offsetY = (float)(ScreenY + apparentSize + 15);
        float offsetX = ScreenX - size.X / 2.0f;
        Raylib.DrawTextEx(Raylib.GetFontDefault(), Name, new Vector2(offsetX, offsetY), calcFontSize, uiScale * 4.5f, Color.White);
    }
    
    private int ScreenX => (int)ScreenXY.X;
    private int ScreenY => (int)ScreenXY.Y;

    private Vector2d ScreenXY
    {
        get => cameraHandler.GetWorldToScreen(Position * worldScale).XY;
    }

    public override string ToString()
    {
        return $"Name: {Name}\nPosition: {Position}\nVelocity: {Velocity}\nMass: {Mass}\nRadius: {Radius}\nColor: {Color}\nScreen Position: {ScreenXY}";
    }
}