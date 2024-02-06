namespace GaltonBoard.Core.Events;

public class FinishedEventArgs(double time) : EventArgs
{
    public double Time { get; } = time;
}