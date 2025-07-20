namespace GravSim;

public class Time : SIUnit
{
    private Time(double magnitude) => Magnitude = magnitude;
    public static Time FromSeconds(double seconds) => new Time(seconds);
    
}