namespace GaltonBoard.Model.Models;

public class RenderParticle
{
    public double PositionX { get; set; }
    public double PositionY { get; set; }
    public double Radius { get; set; }
    public int Color { get; set; }

    public RenderParticle(Particle particle, Border imageSize, Border engineSize)
    {
        var ratioX = imageSize.Width / engineSize.Width;
        var ratioY = imageSize.Height / engineSize.Height;

        var x = particle.Position.X * ratioX + imageSize.MarginWidth;
        var y = particle.Position.Y * ratioY + imageSize.MarginHeight;
        var radius = particle.Config.Radius * ratioX;

        PositionX = x;
        PositionY = Math.Abs(y - imageSize.Height);
        Radius = radius;
        Color = (int)particle.Type;
    }
}