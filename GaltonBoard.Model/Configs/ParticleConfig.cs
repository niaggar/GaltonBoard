namespace GaltonBoard.Model.Configs;

public class ParticleConfig
{
    public float Mass { get; set; }
    public float InverseMass { get; set; }
    public float Radius { get; set; }
    public float Restitution { get; set; }
    public bool IsStatic { get; set; }
    public bool IsInactive { get; set; } = false;
}