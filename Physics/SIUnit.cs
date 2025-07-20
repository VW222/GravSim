namespace GravSim;

public abstract class SIUnit
{
    protected double Magnitude = 0.0;

    public virtual (int, string) StrRep()
    {
        Dictionary<int, char> dict = new()
        {
            {30, 'Q'},
            {27, 'R'},
            {24, 'Y'},
            {21, 'Z'},
            {18, 'E'},
            {15, 'P'},
            {12, 'T'},
            {9, 'G'},
            {6, 'M'},
            {3, 'k'},
            {-3, 'm'},
            {-6, 'µ'},
            {-9, 'n'},
            {-12, 'p'},
            {-15, 'f'},
            {-18, 'a'},
            {-21, 'z'},
            {-24, 'y'}
        };

        if (Magnitude == 0)
        {
            return (0, "");
        }

        var exponent = (int)(Math.Floor(Math.Log10(Math.Abs(Magnitude)) / 3.0) * 3);
        return (exponent, dict.TryGetValue(exponent, out var value) ? value.ToString() : "");
    }
}