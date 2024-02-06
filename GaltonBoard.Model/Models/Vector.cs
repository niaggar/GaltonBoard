namespace GaltonBoard.Model.Models;

public class Vector(double x, double y)
{
    public double X { get; set; } = x;
    public double Y { get; set; } = y;

    public static Vector operator +(Vector a, Vector b)
    {
        return new Vector(a.X + b.X, a.Y + b.Y);
    }

    public static Vector operator -(Vector a, Vector b)
    {
        return new Vector(a.X - b.X, a.Y - b.Y);
    }

    public static Vector operator *(Vector a, double b)
    {
        return new Vector(a.X * b, a.Y * b);
    }

    public static Vector operator *(double b, Vector a)
    {
        return new Vector(a.X * b, a.Y * b);
    }

    public static Vector operator /(Vector a, double b)
    {
        return new Vector(a.X / b, a.Y / b);
    }

    public Vector Normalized()
    {
        var magnitude = Magnitude();
        if (magnitude == 0) return Vector.Zero;

        return this / Magnitude();
    }

    public double Magnitude()
    {
        return Math.Sqrt(MagnitudeSqr());
    }

    public double MagnitudeSqr()
    {
        return X * X + Y * Y;
    }

    public static double Dot(Vector a, Vector b)
    {
        return a.X * b.X + a.Y * b.Y;
    }

    public static Vector Zero => new Vector(0, 0);
    public static Vector Up => new Vector(0, 1);
    public static Vector Down => new Vector(0, -1);
    public static Vector Left => new Vector(-1, 0);
    public static Vector Right => new Vector(1, 0);
}