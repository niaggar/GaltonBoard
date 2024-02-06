using GaltonBoard.Model.Enums;
using GaltonBoard.Model.Models;

namespace GaltonBoard.Model.Configs;

public class PegCreationConfig
{
    public Range<double> Radio { get; set; }
    public Range<double> Mass { get; set; }

    public DirectionEnum Direction { get; set; }

    public double Restitution { get; set; }
    public bool IsStatic { get; set; }

    public float XAmplitude { get; set; }
    public float XFrequency { get; set; }
    public float YAmplitude { get; set; }
    public float YFrequency { get; set; }

    public static PegCreationConfig Default => new()
    {
        Radio = Range<double>.CreateMinMax(1, 2),
        Mass = Range<double>.CreateMinMax(1, 2),
        Direction = DirectionEnum.None,
        Restitution = 0.6,
        IsStatic = true,
        XAmplitude = 0,
        XFrequency = 0,
        YAmplitude = 0,
        YFrequency = 0,
    };
}