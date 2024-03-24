using GaltonBoard.Model.Models;

namespace GaltonBoard.Model.Configs;

public class BallCreationConfig
{
    public int NumberOfBalls { get; set; }
    public int CreationStepInterval { get; set; }
    public double Restitution { get; set; }

    public Range<double> Radio { get; set; }
    public Range<double> Mass { get; set; }

    public Range<double> CenterX { get; set; }
    public Range<double> CenterY { get; set; }

    public Range<double> InitialVelocityX { get; set; }
    public Range<double> InitialVelocityY { get; set; }

    public Range<double> VelocityAngleRange { get; set; }
    public double VelocityMagnitude { get; set; }

    public static BallCreationConfig Default => new()
    {
        NumberOfBalls = 10,
        CreationStepInterval = 1,
        Restitution = 0.3,
        Radio = Range<double>.CreateMinMax(1, 2),
        Mass = Range<double>.CreateMinMax(1, 2),
        CenterX = Range<double>.CreateMinMax(0.5, 0.5),
        CenterY = Range<double>.CreateMinMax(0.9, 1),
        InitialVelocityX = Range<double>.CreateMinMax(-10, 10),
        InitialVelocityY = Range<double>.CreateMinMax(0, 0),
        VelocityAngleRange = Range<double>.CreateMinMax(0, 0),
        VelocityMagnitude = 10,
    };
}