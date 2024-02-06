using GaltonBoard.Model.Models;

namespace GaltonBoard.Core.Events;

public class ParticleCollisionEventArgs(Particle particleA, Particle particleB) : EventArgs
{
    public Particle ParticleA { get; set; } = particleA;
    public Particle ParticleB { get; set; } = particleB;
}