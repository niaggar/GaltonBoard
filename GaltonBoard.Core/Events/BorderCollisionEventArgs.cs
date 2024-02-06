using GaltonBoard.Model.Enums;
using GaltonBoard.Model.Models;

namespace GaltonBoard.Core.Events;

public class BorderCollisionEventArgs(Particle particle, BorderEnum border) : EventArgs
{
    public Particle Particle { get; set; } = particle;
    public BorderEnum Border { get; } = border;
}