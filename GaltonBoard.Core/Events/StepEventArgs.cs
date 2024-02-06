using GaltonBoard.Model.Models;

namespace GaltonBoard.Core.Events;

public class StepEventArgs(int currentStep, float currentTime, Particle[]? particles) : EventArgs
{
    public int CurrentStep { get; set; } = currentStep;
    public float CurrentTime { get; set; } = currentTime;
    public Particle[]? Particles { get; set; } = particles;
}