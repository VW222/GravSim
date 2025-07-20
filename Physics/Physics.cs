using Raylib_cs;

namespace GravSim;

public static class Physics
{
    public const double G = 6.6743015e-11;

    public static Vector3d NewtonianGravity(Planet body1, Planet body2)
    {
        const double epsilonSquared = 1e-10;
        
        Vector3d direction = body2.Position - body1.Position;
        double distanceSquared = direction.LengthSquared() + epsilonSquared;
        
        if (distanceSquared < epsilonSquared) return new  Vector3d(0, 0, 0);

        // F = G * m1 * m2 / r²
        double forceMagnitude = G * body1.Mass.GetKg() * body2.Mass.GetKg() / distanceSquared;
        return direction * (forceMagnitude / Math.Sqrt(distanceSquared));
    }

    public static double EaseInOutCubic(double x)
    {
        return x < 0.5 ? 4f * x * x * x : 1 - Math.Pow(-2f * x + 2f, 3f) / 2f;
    }

    public static double CubicInterpolate(float l1, float l2, float factor)
    {
        return EaseInOutCubic(factor) * (l2 - l1) + l1;
        // [0-1] -> [l1, l2]
    }

    public static Vector2d CubicInterpolate(Vector2d l1, Vector2d l2, float factor)
    {
        var x = l1.X + (l2.X - l1.X) * EaseInOutCubic(factor);
        var y = l1.Y + (l2.Y - l1.Y) * EaseInOutCubic(factor);
        return new Vector2d(x, y);
    }

    public static Vector3d CubicInterpolate(Vector3d l1, Vector3d l2, float factor)
    {
        var x = l1.X + (l2.X - l1.X) * EaseInOutCubic(factor);
        var y = l1.Y + (l2.Y - l1.Y) * EaseInOutCubic(factor);
        var z = l1.Z + (l2.Z - l1.Z) * EaseInOutCubic(factor);
        return new Vector3d(x, y, z);
    }

    public static (Vector3d, Vector3d) findTangencyPoints(Vector3d P, Vector3d C, double R)
    {
        Vector3d CP = P - C;

        double d = Vector3d.Distance(C, P);

        Vector3d CP_normalized = CP.Normalized();

        Vector3d k_hat = new Vector3d(0, 0, 1);
        Vector3d T = Raymath.Vector3CrossProduct(CP, k_hat);
        Vector3d T_normalized = T.Normalized();

        Vector3d A = new(P.X + R * CP_normalized.X + R * T_normalized.X,
            P.Y + R * CP_normalized.Y + R * T_normalized.Y,
            P.Z + R * CP_normalized.Z + R * T_normalized.Z);

        Vector3d B = new(P.X + R * CP_normalized.X - R * T_normalized.X,
            P.Y + R * CP_normalized.Y - R * T_normalized.Y,
            P.Z + R * CP_normalized.Z - R * T_normalized.Z);

        return (A, B);
    }

    public static double[,] YawMatrix(double alpha)
    {
        double[,] mat = new double[3, 3];
        
        mat[0, 0] = double.Cos(alpha);
        mat[1, 0] = double.Sin(alpha);
        mat[0, 1] = -double.Sin(alpha);
        mat[1, 1] = double.Cos(alpha);
        mat[2, 2] = 1;
        
        return mat;
    }
    
    public static double[,] PitchMatrix(double beta)
    {
        double[,] mat = new double[3, 3];
        
        mat[0, 0] = double.Cos(beta);
        mat[2, 0] = -double.Sin(beta);
        mat[0, 2] = double.Sin(beta);
        mat[2, 2] = double.Cos(beta);
        mat[1, 1] = 1;
        
        return mat;
    }
    
    public static double[,] MatrixMultiply(double[,] A, double[,] B)
    {

        int aRows = A.GetLength(0);
        int aColumns = A.GetLength(1);
        int bRows = B.GetLength(0);
        int bColumns = B.GetLength(1);

        if (aColumns != bRows)
        {
            throw new ArgumentException("A:Rows: " + aColumns + " did not match B:Columns " + bRows + ".");
        }

        double[,] C = new double[aRows, bColumns];

        for (int i = 0; i < aRows; i++)
        { // aRow
            for (int j = 0; j < bColumns; j++)
            { // bColumn
                for (int k = 0; k < aColumns; k++)
                { // aColumn
                    C[i, j] += A[i, k] * B[k, j];
                }
            }
        }

        return C;
    }
}