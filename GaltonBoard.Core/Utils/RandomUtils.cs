using GaltonBoard.Model.Models;

namespace GaltonBoard.Core.Utils;

public static class RandomUtils
{
    private static Random Random { get; } = new Random();

    public static double NextDouble(Range<double> range)
    {
        return Random.NextDouble() * (range.Max - range.Min) + range.Min;
    }

    public static int GetRandomInt()
    {
        return Random.Next();
    }
}