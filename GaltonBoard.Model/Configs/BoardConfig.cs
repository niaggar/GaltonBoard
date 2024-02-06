using GaltonBoard.Model.Models;

namespace GaltonBoard.Model.Configs;

public class BoardConfig
{
    public int NumberOfColumns { get; set; }
    public int NumberOfRows { get; set; }

    public double MarginDown { get; set; }
    public double MarginUp { get; set; }
    public double MarginSides { get; set; }

    public double Restitution { get; set; }
    public double ResizeFactorX { get; set; }
    public double ResizeFactorY { get; set; }

    public double RowsHeight { get; set; }
    public double ColumnsWidth { get; set; }

    public static BoardConfig Default => new()
    {
        NumberOfColumns = 10,
        NumberOfRows = 10,
        MarginDown = 5,
        MarginUp = 10,
        MarginSides = 0,
        Restitution = 0.6,
        ResizeFactorX = 1,
        ResizeFactorY = 1,
        RowsHeight = 4,
        ColumnsWidth = 4,
    };
}