using System.Runtime.CompilerServices;
using GaltonBoard.Model.Configs;
using GaltonBoard.Model.Enums;

namespace GaltonBoard.Model.Models;

public class Peg(ParticleConfig config, PegConfig pegConfig) : Particle(config)
{
    public PegConfig PegConfig { get; set; } = pegConfig;
    private float CurrentTime { get; set; }
    public override ParticleEnum Type => ParticleEnum.Peg;

    private Vector PreviousSin { get; set; } = Vector.Zero;

    public override void Update(float deltaTime)
    {
        if (Config.IsStatic || Config.IsInactive) return;
        CurrentTime += deltaTime;

        var xFunction = Sinenoidal(CurrentTime, PegConfig.XFrequency, PegConfig.XAmplitude);
        var yFunction = Sinenoidal(CurrentTime, PegConfig.YFrequency, PegConfig.YAmplitude);

        var newDisplacement = PreviousSin - new Vector(xFunction, yFunction);
        var xVelocity = Cosenoidal(CurrentTime, PegConfig.XFrequency, PegConfig.XAmplitude * PegConfig.XFrequency);
        var yVelocity = Cosenoidal(CurrentTime, PegConfig.YFrequency, PegConfig.YAmplitude * PegConfig.YFrequency);

        Velocity += new Vector(xVelocity, yVelocity);
        Position += newDisplacement;
        PreviousSin = new Vector(xFunction, yFunction);
    }

    private static float Cosenoidal(float x, float frequency, float amplitude)
    {
        return (float) Math.Cos(x * frequency) * amplitude;
    }

    private static float Sinenoidal(float x, float frequency, float amplitude)
    {
        return (float) Math.Sin(x * frequency) * amplitude;
    }
}