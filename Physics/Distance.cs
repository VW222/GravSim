namespace GravSim;

public class Distance : SIUnit
{
    private Distance(double magnitude) => Magnitude = magnitude;

    public static Distance FromSolarRadii(double solarRadii) => new Distance(solarRadii * SolarRadius);
    public static Distance FromAU(double au) => new Distance(au * AU);
    public static Distance FromMeters(double m) => new Distance(m);
    public double GetMeters() => Magnitude;
    public const long AU = 149597870700;
    public const double SolarRadius = 695700000d;
    public override string ToString() => $"{(Magnitude / Math.Pow(10, StrRep().Item1)):f2} {StrRep().Item2}m";

    public static Distance operator *(Distance lh, double rh) => new(lh.Magnitude * rh);
    public static Distance operator /(Distance lh, double rh) => new(lh.Magnitude / rh);
}