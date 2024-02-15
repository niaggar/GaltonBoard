using GaltonBoard.Model.Enums;

namespace GaltonBoard.Model.Models;

public class Cell
{
    public List<Ball> Balls = new();
    public List<Peg> Pegs = new();

    public void AddParticle(Particle peg, ParticleEnum particleType)
    {
        if (particleType == ParticleEnum.Ball)
        {
            Balls.Add((Ball)peg);
        }
        else
        {
            Pegs.Add((Peg)peg);
        }
    }

    public void Clear()
    {
        Balls.Clear();
        Pegs.Clear();
    }
}