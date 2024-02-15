namespace GaltonBoard.Model.Models;

public static class Constants
{
    public const float Gravity = 9.81f;
    public const bool IsDebug = false;
    public const bool SolveUsingMomentum = false;
    public const bool SolveUsingImpulse = true;
    public const double RadiusBorderScale = 0.2;
    public const int StepsToNotify = 20;

    public const int GridBoardColumns = 10;
    public const int GridBoardRows = 10;

    public const int NumberOfSimultaneousCollisionsValidation = 4;
    public const bool IsParallel = true;
}