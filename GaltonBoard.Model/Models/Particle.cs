using GaltonBoard.Model.Configs;
using GaltonBoard.Model.Enums;

namespace GaltonBoard.Model.Models;

public abstract class Particle(ParticleConfig config)
{
    public int Id { get; set; }
    public Vector Position { get; set; } = Vector.Zero;
    public Vector Velocity { get; set; } = Vector.Zero;
    public Vector Force { get; set; } = Vector.Zero;

    public ParticleConfig Config { get; set; } = config;
    public abstract ParticleEnum Type { get; }

    public void ApplyForce(Vector force)
    {
        if (Config.IsStatic || Config.IsInactive) return;
        if (Type == ParticleEnum.Peg) return;

        Force += force;
    }

    public void ApplyDrag(float drag)
    {
        if (Config.IsStatic || Config.IsInactive) return;
        if (Type == ParticleEnum.Peg) return;

        var force = (Velocity * -1).Normalized();
        force *= drag;
        ApplyForce(force);
    }

    public abstract void Update(float deltaTime);
}