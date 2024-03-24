using GaltonBoard.Model.Configs;
using GaltonBoard.Model.Models;

namespace GaltonBoard.Core.Utils;

public static class BorderFactory
{
    public static Border CreateBorder(Particle[] pegs, BoardConfig borderConfig)
    {
        var minX = pegs.Min(p => p.Position.X);
        var distanceToLeft = borderConfig.MarginSides - minX;

        pegs.ToList().ForEach(p => p.Position.X += distanceToLeft);

        var boardSize = new Vector(
            pegs.Max(p => p.Position.X) + borderConfig.MarginSides,
            pegs.Max(p => p.Position.Y) + borderConfig.MarginUp + borderConfig.MarginDown
        );

        return new Border
        {
            Width = boardSize.X,
            Height = boardSize.Y,
            Restitution = borderConfig.Restitution,
        };
    }

    public static Vector[] CreateDrawBorder(Border border)
    {
        var numberOfVerticalPoints = border.Height / (Constants.RadiusBorderScale * 2);
        var numberOfHorizontalPoints = border.Width / (Constants.RadiusBorderScale  * 2);

        var points = new List<Vector>();

        // for (var i = 0; i < numberOfVerticalPoints; i++)
        // {
        //     points.Add(new Vector(0, i * Constants.RadiusBorderScale  * 2));
        //     points.Add(new Vector(border.Width, i * Constants.RadiusBorderScale  * 2));
        // }
        //
        // for (var i = 0; i < numberOfHorizontalPoints; i++)
        // {
        //     points.Add(new Vector(i * Constants.RadiusBorderScale  * 2, 0));
        //     points.Add(new Vector(i * Constants.RadiusBorderScale  * 2, border.Height));
        // }

        return points.ToArray();
    }
}