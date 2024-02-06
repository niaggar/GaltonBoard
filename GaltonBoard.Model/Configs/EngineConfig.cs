using GaltonBoard.Model.Models;

namespace GaltonBoard.Model.Configs;

public class EngineConfig
{
    public Vector Gravity { get; set; }
    public Border Border { get; set; }
    public double Drag { get; set; }
    public bool IsCollisionActive { get; set; }
    public bool IsPaused { get; set; }

    public static EngineConfig Default => new ()
    {
        Gravity = new GaltonBoard.Model.Models.Vector(0,-9.81),
        Border = new Border() { Height = 100, Width = 100, Restitution = 0.6 },
        Drag = 0,
        IsCollisionActive = true,
        IsPaused = false,
    };
}