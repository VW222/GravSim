using System.Numerics;
using System.Xml.Linq;
using Raylib_cs;

namespace GravSim;

public class Vector3d(double x, double y, double z)
{
    public double X = x, Y = y, Z = z;

    public Vector2d XY
    {
        get => new(X, Y);
        set { X = value.X; Y = value.Y; }
    }
    public Vector2d XZ
    {
        get => new(X, Z);
        set { X = value.X; Z = value.Y; }
    }
    public Vector2d YZ
    {
        get => new(Y, Z);
        set { Y = value.X; Z = value.Y; }
    }

    public static Vector3d operator +(Vector3d v1, Vector3d v2)
    {
        return new Vector3d(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
    }

    public static Vector3d operator -(Vector3d v1, Vector3d v2)
    {
        return new Vector3d(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
    }

    public static Vector3d operator *(Vector3d v1, double v2)
    {
        return new Vector3d(v1.X * v2, v1.Y * v2, v1.Z * v2);
    }

    public static Vector3d operator /(Vector3d v1, double v2)
    {
        return new Vector3d(v1.X / v2, v1.Y / v2, v1.Z / v2);
    }
    public static Vector3d operator /(Vector3d v1, Distance v2)
    {
        return new Vector3d(v1.X / v2.GetMeters(), v1.Y / v2.GetMeters(), v1.Z / v2.GetMeters());
    }
    public static Vector3d operator *(Vector3d v1, Distance v2)
    {
        return new Vector3d(v1.X * v2.GetMeters(), v1.Y * v2.GetMeters(), v1.Z * v2.GetMeters());
    }
    public double LengthSquared() => X * X + Y * Y + Z * Z;
    
    public Vector3d Normalized()
    {
        var length = Math.Sqrt(LengthSquared());
        return length == 0 ? new Vector3d(0, 0, 0) : new Vector3d(X / length, Y / length,Z / length);
    }

    public void Normalize()
    {
        var length = Math.Sqrt(LengthSquared());
        X /= length;
        Y /= length;
        Z /= length;
    }
    
    public static double Distance(Vector3d v1, Vector3d v2)
    {
        return Math.Cbrt(Math.Pow(v2.X - v1.X, 2) + Math.Pow(v2.Y - v1.Y, 2) + Math.Pow(v2.Z - v1.Z, 2));
    }

    public override string ToString() => $"<{X:f2}, {Y:f2}, {Z:f2}>";

    public static Vector3d Deserialize(XElement? xElement)
    {
        if (xElement == null) return new Vector3d(0, 0, 0);
        var x = xElement.Element("X")?.Value ?? "0";
        var y = xElement.Element("Y")?.Value ?? "0";
        var z = xElement.Element("Z")?.Value ?? "0";
        return new Vector3d(Convert.ToDouble(x), Convert.ToDouble(y), Convert.ToDouble(z));
    }

    public static implicit operator Color(Vector3d v) => new((float)v.X, (float)v.Y, (float)v.Z);
    public static implicit operator Vector3(Vector3d v) => new((float)v.X, (float)v.Y, (float)v.Z);
    public static implicit operator Vector3d(Vector3 v) => new(v.X, v.Y, v.Z);
    public static implicit operator Vector3d(Vector2d v) => new(v.X, v.Y, 0.0f);
    public static implicit operator Vector3d(Vector2 v) => new(v.X, v.Y, 0.0f);
    public static implicit operator double[,](Vector3d v)
    {
        double[,] vmat = new double[1, 3];
        vmat[0, 0] = v.X;
        vmat[0, 1] = v.Y;
        vmat[0, 2] = v.Z;
        return vmat;
    }
    public static implicit operator Vector3d(double[,] mat) => new(mat[0, 0], mat[0, 1], mat[0, 2]);

    public static Vector3d operator *(Vector3d v1, double[,] m) => Physics.MatrixMultiply(v1, m);
    public static Vector3d Zero => new(0, 0, 0);
}