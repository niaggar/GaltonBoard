using GaltonBoard.Model.Configs;
using GaltonBoard.Model.Enums;

namespace GaltonBoard.Model.Models;

public class Peg(ParticleConfig config, PegConfig pegConfig) : Particle(config)
{
    public PegConfig PegConfig { get; set; } = pegConfig;
    private float CurrentTime { get; set; }
    public override ParticleEnum Type => ParticleEnum.Peg;

    public override void Update(float deltaTime)
    {
        if (Config.IsStatic || Config.IsInactive) return;
        CurrentTime += deltaTime;

        var xFunction = Cosenoidal(CurrentTime, PegConfig.XFrequency, PegConfig.XAmplitude);
        var yFunction = Cosenoidal(CurrentTime, PegConfig.YFrequency, PegConfig.YAmplitude);

        var xVelocity = Sinenoidal(CurrentTime, PegConfig.XFrequency, PegConfig.XAmplitude * PegConfig.XFrequency);
        var yVelocity = Sinenoidal(CurrentTime, PegConfig.YFrequency, PegConfig.YAmplitude * PegConfig.YFrequency);

        Velocity += new Vector(xVelocity, yVelocity);
        Position += new Vector(xFunction, yFunction);
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