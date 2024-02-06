namespace GaltonBoard.Model.Models;

public class Range<T> where T : IComparable, IComparable<T>
{
    public T Center { get; set; } = default!;
    public T Width { get; set; } = default!;

    public T Min => Subtract(Center, Divide(Width, 2));
    public T Max => Add(Center, Divide(Width, 2));

    public static Range<T> Create(T center, T width)
    {
        return new Range<T>
        {
            Center = center,
            Width = width
        };
    }

    public static Range<T> CreateMinMax(T min, T max)
    {
        return new Range<T>
        {
            Center = Divide(Add(min, max), 2),
            Width = Subtract(max, min)
        };
    }

    private static T Add(T left, T right)
    {
        dynamic a = left;
        dynamic b = right;
        return a + b;
    }

    private static T Subtract(T left, T right)
    {
        dynamic a = left;
        dynamic b = right;
        return a - b;
    }

    private static T Divide(T left, int right)
    {
        dynamic a = left;
        dynamic b = right;
        return a / b;
    }
}