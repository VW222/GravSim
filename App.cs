using System.Xml.Linq;
using Raylib_cs;

namespace GravSim;

public class App
{
    private const double Scale = 200.0 / Distance.AU; // Scale is 1f in 3d (for now)
    private readonly List<Planet> bodies = [];
    private Vector3d center = new Vector3d(0, 0, 0);
    private int frameCount;
    private bool paused;
    private const int TargetFps = 200;
    private int followIndex;

    private float uiScale = 1.25f;
    // private float _panSensitivity = 1f;
    private bool isF3Visible = true;
    private Vector2d screenSize = new Vector2d(2580, 1080);

    private bool doPan;

    private Planet SelectedPlanet => bodies[followIndex];

    // private Distance _bodyRelativeDistance = Distance.FromAU(20f);
    // private Vector3d _bodyRelativeLookAngle = new Vector3d(0, 4, 1).Normalized();
    // multiply by body->camera vector (normalized)

    private double timeScale = 31556926.0;
    private CameraHandler cameraHandler;
    private Texture2D bounds;

    private int steps = 100;

    public App()
    {
        InitWindow();
        AddCelObjects();
        if (bodies.Count == 0)
        {
            throw new Exception("Planets.xml couldnt be loaded");
        }

        bounds = Raylib.LoadTexture("Assets/bounds.png");

        cameraHandler = new CameraHandler2D(screenSize);

        RenderScene();
        Raylib.CloseWindow();
    }

    private static Planet Deserialize(XElement xElement) =>
        new()
        {
            Name = xElement.Element("Name")?.Value ?? "",
            Mass = Mass.FromEarth(Convert.ToDouble(xElement.Element("Mass")?.Value)),
            Position = Vector3d.Deserialize(xElement.Element("Position")) * Distance.AU,
            Radius = Distance.FromSolarRadii(Convert.ToDouble(xElement.Element("Radius")?.Value)),
            TrailLength = Convert.ToInt32(xElement.Element("TrailLength")?.Value ?? "50"),
            Color = Vector3d.Deserialize(xElement.Element("Color")),
            Immovable = Convert.ToBoolean(xElement.Element("Immovable")?.Value),
            Velocity = Vector3d.Deserialize(xElement.Element("Velocity")),
            LabelMultiplier = Convert.ToSingle(xElement.Element("LabelMultiplier")?.Value),
        };

    private void AddCelObjects()
    {
        XElement solar = XElement.Load(@"Planets.xml");
        foreach (var xElement in solar.Elements())
        {
            var planet = Deserialize(xElement);
            bodies.Add(planet);
        }
    }

    private void InitWindow()
    {
        // Raylib.SetConfigFlags(ConfigFlags.ResizableWindow); // | ConfigFlags.Msaa4xHint
        Raylib.InitWindow((int)screenSize.X, (int)screenSize.Y, "GravSim");
        Raylib.SetTargetFPS(TargetFps);

        // _center = new Vector3d(Raylib.GetScreenWidth() / 2.0, Raylib.GetScreenHeight() / 2.0);
    }

    private void RenderScene()
    {
        Shader shader = Raylib.LoadShader(null, "Shaders/smooth.frag");
        RenderTexture2D target = Raylib.LoadRenderTexture((int)screenSize.X, (int)screenSize.Y);

        while (!Raylib.WindowShouldClose())
        {
            var fps = Raylib.GetFPS();
            HandleEvents();
            steps = Math.Max((int)(timeScale / 31556926 * 150), 1);
            SimulatePhysics(1.0d / (fps != 0 ? fps : TargetFps));

            var erth = bodies[3];
            var mun = bodies[4];
            var dist = Math.Sqrt((erth.Position - mun.Position).LengthSquared());
            Console.WriteLine(Distance.FromMeters(dist));

            // _camera.Offset = _camera.Target * _camera.Zoom;
            foreach (var body in bodies)
            {
                body.Update(center, Scale, cameraHandler, uiScale);
            }
            cameraHandler.MoveTo(SelectedPlanet.Position * Scale);
            cameraHandler.AnimationStep(1.0f / fps);
            
            Render(target, shader);
            frameCount++;
        }

        Raylib.UnloadRenderTexture(target);
        Raylib.UnloadShader(shader);
    }

    private void Render(RenderTexture2D target, Shader shader)
    {
        // Raylib.BeginTextureMode(target);
        // {
        //     Raylib.ClearBackground(Color.Black);
        //     foreach (var body in _bodies)
        //     {
        //         body.Draw();
        //         // SelectedPlanet.DrawBounds(_bounds);
        //     }
        // }
        // Raylib.EndTextureMode();

        Raylib.BeginDrawing();
        {
            // Raylib.BeginShaderMode(shader);
            // // Raylib.DrawTexturePro(target.Texture, new Rectangle(0, 0, target.Texture.Width, -target.Texture.Height),
            // //     new Rectangle(0.0f, 0.0f, (float)_screenSize.X, (float)_screenSize.Y), new Vector2(),
            // //     0.0f, Color.White);
            // Raylib.DrawTextureV(target.Texture, new Vector2(0, 0), Color.White);
            // Raylib.EndShaderMode();
            Raylib.ClearBackground(Color.Black);
            // _cameraHandler.BeginMode();
            foreach (var body in bodies)
            {
                body.Draw();
                SelectedPlanet.DrawBounds(bounds);
            }
            
            // _cameraHandler.EndMode();

            String textBuffer = "";
            textBuffer += $"FPS: {Raylib.GetFPS()}\n";
            textBuffer += $"Frame {frameCount}\n";
            textBuffer += $"Time Scale: {TimeSpan.FromSeconds(timeScale).TotalDays:f2}days/s [E/Q]\n";
            textBuffer += $"Simulation Steps: {steps}\n";
            
            // textBuffer += $"Target Zoom: {_targetZoom:f0} [Y]\n";
            textBuffer += $"Follow Target: {SelectedPlanet.Name} [L/R Arrow]\n\n";

            textBuffer += $"Target Properties\n{SelectedPlanet}\n\n";

            textBuffer += $"Camera Properties\n";
            textBuffer += $"{cameraHandler}\n\n";

            textBuffer += $"UI Scale: {uiScale:f2}x [N/M]\n";
            textBuffer += $"Toggle F3 Menu: [F3]\n";

            if (isF3Visible)
            {
                Raylib.DrawTextEx(Raylib.GetFontDefault(), textBuffer, new Vector2d(10, 10), 20.0f * uiScale,
                    3.0f * uiScale, Color.White);
            }
        }
        Raylib.EndDrawing();
    }

    // private string LookAngleDegrees() =>
    //     $"<{(Math.Asin(_bodyRelativeLookAngle.X) * 180d / Math.PI):f2}°, {(Math.Asin(_bodyRelativeLookAngle.Y) * 180d / Math.PI):f2}°, {(Math.Asin(_bodyRelativeLookAngle.Z) * 180d / Math.PI):f2}°>";

    private void HandleEvents()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Space))
        {
            paused = !paused;
        }

        if (Raylib.IsKeyPressed(KeyboardKey.F3))
        {
            isF3Visible = !isF3Visible;
        }

        // if (Raylib.IsKeyPressed(KeyboardKey.X))
        // {
        //     _doSnap = !_doSnap;
        // }

        if (Raylib.IsKeyPressed(KeyboardKey.E))
        {
            timeScale *= 2;
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Q))
        {
            timeScale /= 2;
        }

        // if (Raylib.IsKeyPressed(KeyboardKey.T))
        // {
        //     _movementInterpolationFactor *= 1.25f;
        // }
        //
        // if (Raylib.IsKeyPressed(KeyboardKey.R))
        // {
        //     _movementInterpolationFactor /= 1.25f;
        // }

        if (Raylib.IsKeyPressed(KeyboardKey.N))
        {
            uiScale -= 0.05f;
        }

        if (Raylib.IsKeyPressed(KeyboardKey.M))
        {
            uiScale += 0.05f;
        }

        if (Raylib.IsMouseButtonPressed(MouseButton.Right))
        {
            Raylib.HideCursor();
            doPan = true;
        }
        
        if (Raylib.IsMouseButtonReleased(MouseButton.Right))
        {
            Raylib.ShowCursor();
            doPan = false;
        }

        uiScale = Math.Clamp(uiScale, 0.5f, 3f);

        cameraHandler.MovementInterpolationFactor = Math.Clamp(cameraHandler.MovementInterpolationFactor, 0.05f, 1f);

        if (Raylib.IsKeyPressed(KeyboardKey.Right))
        {
            followIndex++;
            if (followIndex >= bodies.Count) followIndex = 0;
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Left))
        {
            followIndex--;
            if (followIndex < 0) followIndex = bodies.Count - 1;
        }

        if (doPan)
        {
            Vector2d move = Raylib.GetMouseDelta() / 50f;
            // _bodyRelativeLookAngle.X += move.X;
            // _bodyRelativeLookAngle.Z += move.Y;
            // Console.WriteLine(Physics.MatrixMultiply(Physics.YawMatrix(move.X), Physics.PitchMatrix(move.Y)));
            // _bodyRelativeLookAngle *= Physics.MatrixMultiply(Physics.YawMatrix(move.X), Physics.PitchMatrix(move.Y));
            cameraHandler.PanInput(move * -1);
            Raylib.SetMousePosition((int)(screenSize.X/2d), (int)(screenSize.Y/2d)); 
        }

        // _cameraTargetTarget = SelectedPlanet.Position;
        // _cameraTargetPosition = SelectedPlanet.Position + _bodyRelativeLookAngle * _bodyRelativeDistance;
        //
        // // if (_isFollowing && _bodies.Count > 0) _cameraTargetPosition = SelectedPlanet.GetScreenPosition();
        // _targetFovY = MathF.Exp(MathF.Log(_targetFovY) + Raylib.GetMouseWheelMove() * -0.025f);
        // _targetFovY = Math.Clamp(_targetFovY, 0f, 179.69f);
        // _camera.FovY = (float)Physics.CubicInterpolate(_camera.FovY, _targetFovY, _movementInterpolationFactor);
        // _camera.Position = Physics.CubicInterpolate(_camera.Position, _cameraTargetPosition,
        //     _movementInterpolationFactor);
        // _camera.Target = Physics.CubicInterpolate(_camera.Target, _cameraTargetTarget,
        //     _movementInterpolationFactor);
        // // _camera.Target = _bodies[0].Position;
        var scrollDelta = Raylib.GetMouseWheelMove();
        if (scrollDelta != 0)
        {
            cameraHandler.ScrollInput(Raylib.GetMouseWheelMove() * 0.075f); //* -0.025f);
        }
    }

    private void SimulatePhysics(double deltaTime)
    {
        if (paused) return;
        double stepSize = deltaTime * timeScale / steps;
        var forces = new Vector3d[bodies.Count];

        for (int step = 0; step < steps; step++)
        {
            // Reset forces
            Array.Fill(forces, Vector3d.Zero);
            
            for (int index = 0; index < bodies.Count; index++)
            {
                var localForce = Vector3d.Zero;

                for (int j = index + 1; j < bodies.Count; j++)
                {
                    var force = Physics.NewtonianGravity(bodies[index], bodies[j]);
                    localForce += force;
                    forces[j] -= force;
                }

                forces[index] += localForce;
            };

            // Update positions and velocities
            for (int j = 0; j < bodies.Count; j++)
            {
                if (bodies[j].Immovable) continue;

                var acceleration = forces[j] / bodies[j].Mass.GetKg();

                // Velocity Verlet: Update position
                bodies[j].Position += bodies[j].Velocity * stepSize + acceleration * 0.5 * stepSize * stepSize;

                // Recalculate forces for the updated position
                forces[j] = Vector3d.Zero;
                for (int k = 0; k < bodies.Count; k++)
                {
                    if (j == k || (bodies[j].Immovable && bodies[k].Immovable)) continue;
                    forces[j] += Physics.NewtonianGravity(bodies[j], bodies[k]);
                }

                var newAcceleration = forces[j] / bodies[j].Mass.GetKg();
                bodies[j].Velocity += (acceleration + newAcceleration) * 0.5 * stepSize;
            }
        }
    }

}