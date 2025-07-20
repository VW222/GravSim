namespace GravSim;

public class Mass : SIUnit
{
    private const double SOLAR_KG = 1.988416e30;
    private const double EARTH_KG = 5.9722e24;

    private Mass(double magnitude)
    {
        Magnitude = magnitude;
    }

    public double GetKg()
    {
        return Magnitude;
    }

    public static Mass FromKg(double kg)
    {
        return new Mass(kg);
    }

    public static Mass FromSolar(double solar)
    {
        return new Mass(solar * SOLAR_KG);
    }

    public static Mass FromEarth(double earth)
    {
        return new Mass(earth * EARTH_KG);
    }
    public override string ToString() => $"{(Magnitude / Math.Pow(10, StrRep().Item1)):f2} {StrRep().Item2}kg";
}