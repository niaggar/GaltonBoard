using GaltonBoard.Model.Configs;
using GaltonBoard.Model.Enums;

namespace GaltonBoard.Model.Models;

public class Ball(ParticleConfig config) : Particle(config)
{
    public override ParticleEnum Type => ParticleEnum.Ball;

    public override void Update(float deltaTime)
    {
        if (Config.IsStatic || Config.IsInactive) return;
        Velocity += Force * deltaTime * Config.InverseMass;
        Position += Velocity * deltaTime;
        Force = Vector.Zero;
    }
}