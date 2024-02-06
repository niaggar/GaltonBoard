using GaltonBoard.Model.Enums;
using GaltonBoard.Model.Models;

namespace GaltonBoard.Core.Logic;

public static class BorderCollider
{
    public static BorderEnum Check(Border border, Vector position)
    {
        if (position.X < 0) return BorderEnum.Left;
        if (position.X > border.Width) return BorderEnum.Right;
        if (position.Y < 0) return BorderEnum.Bottom;
        if (position.Y > border.Height) return BorderEnum.Top;

        return BorderEnum.None;
    }

    public static void Resolve(Border border, BorderEnum borderEnum, Particle particle)
    {
        switch (borderEnum)
        {
            case BorderEnum.Left:
                particle.Position.X = 0 + particle.Config.Radius;
                particle.Velocity.X *= -1 * border.Restitution;
                break;
            case BorderEnum.Right:
                particle.Position.X = border.Width - particle.Config.Radius;
                particle.Velocity.X *= -1 * border.Restitution;
                break;
            case BorderEnum.Bottom:
                particle.Position.Y = 0 + particle.Config.Radius;
                particle.Velocity.Y *= -1 * border.Restitution;
                break;
            case BorderEnum.Top:
                particle.Position.Y = border.Height - particle.Config.Radius;
                particle.Velocity.Y *= -1 * border.Restitution;
                break;
            case BorderEnum.None:
                break;
        }
    }
}