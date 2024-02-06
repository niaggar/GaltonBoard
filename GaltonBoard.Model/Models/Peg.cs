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

        var xFunction = Sinusoidal(CurrentTime, PegConfig.XFrequency, PegConfig.XAmplitude);
        var yFunction = Sinusoidal(CurrentTime, PegConfig.YFrequency, PegConfig.YAmplitude);

        Position += new Vector(xFunction, yFunction);
    }

    private static float Sinusoidal(float x, float frequency, float amplitude)
    {
        return (float) Math.Sin(x * frequency) * amplitude;
    }
}