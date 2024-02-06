namespace GaltonBoard.Model.Configs;

public class TimeConfig
{
    public double TimeStep { get; set; }
    public int SubSteps { get; set; }
    public int MaxSteps { get; set; }

    public static TimeConfig Default => new()
    {
        TimeStep = 0.1,
        SubSteps = 5,
        MaxSteps = 200
    };
}