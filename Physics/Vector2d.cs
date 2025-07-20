using System.Numerics;

namespace GravSim;

public class Vector2d(double x, double y)
{
    public double X = x, Y = y;
    
    public static implicit operator Vector2(Vector2d v) => new((float)v.X, (float)v.Y);
    public static implicit operator (double, double)(Vector2d v) => (v.X, v.Y);
    public static implicit operator Vector2d(Vector2 v) => new(v.X, v.Y);
    public static Vector2d operator *(Vector2d v, double s) => new(v.X * s, v.Y * s);
    public static Vector2d operator /(Vector2d v, double s) => new(v.X / s, v.Y / s);
    public static Vector2d operator +(Vector2d v, Vector2d v2) => new(v.X + v2.X, v.Y + v2.Y);
    public static Vector2d operator -(Vector2d v, Vector2d v2) => new(v.X - v2.X, v.Y - v2.Y);
    public override string ToString() => $"<{X:f2}, {Y:f2}>";
}