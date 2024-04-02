using GaltonBoard.Core.Events;
using GaltonBoard.Model.Configs;
using GaltonBoard.Model.Enums;
using GaltonBoard.Model.Models;

namespace GaltonBoard.Core.Logic;

public class Engine(EngineConfig configs, BoardConfig board, int particlesCount, int pegsCount)
{
    public double CurrentTime { get; set; } = 0.0d;
    public int CurrentStep { get; set; } = 0;
    public int MaxSteps { get; set; } = 0;

    public int BallsCount { get; set; } = particlesCount;
    public int PegsCount { get; set; } = pegsCount;

    public EngineConfig Configs { get; set; } = configs;
    public List<Particle> Particles { get; set; } = new() { Capacity = particlesCount + pegsCount };

    public event EventHandler<BorderCollisionEventArgs>? BorderCollision;
    public event EventHandler<ParticleCollisionEventArgs>? ParticleCollision;
    public event EventHandler<FinishedEventArgs>? Finished;
    public event EventHandler<StepEventArgs>? Step;

    public void AddParticle(Particle particle)
    {
        Particles.Add(particle);
    }

    public void RunStep(float deltaTime, int subSteps = 1)
    {
        if (Configs.IsPaused) return;

        var deltaSubStep = deltaTime / subSteps;
        for (var i = 0; i < subSteps; i++)
        {
            UpdatePegs(deltaSubStep);
            UpdateBalls(deltaSubStep);
            ValidateCollisions();
        }

        CurrentTime += deltaTime;
        CurrentStep++;

        OnStep();
        ValidateFinished();
    }

    private void UpdatePegs(float deltaTime)
    {
        var pegs = Particles.Where(p => p.Type == ParticleEnum.Peg).ToList();
        var pegsCount = pegs.Count;

        Parallel.For(0, pegsCount, (i, state) =>
        {
            var peg = pegs[i];
            if (peg.Config.IsInactive) return;

            peg.Update(deltaTime);
        });
    }

    private void UpdateBalls(float deltaTime)
    {
        var balls = Particles.Where(p => p.Type == ParticleEnum.Ball).ToList();
        var ballsCount = balls.Count;

        Parallel.For(0, ballsCount, (i, state) =>
        {
            var ball = balls[i];
            if (ball.Config.IsInactive) return;

            ball.ApplyDrag((float)Configs.Drag);
            ball.ApplyForce(Configs.Gravity);
            ball.Update(deltaTime);

            var borderCollision = BorderCollider.Check(Configs.Border, ball.Position);
            if (borderCollision == BorderEnum.None) return;

            BorderCollider.Resolve(Configs.Border, borderCollision, ball);
            OnBorderCollision(ball, borderCollision);
        });
    }

    private void ValidateCollisions()
    {
        var pegs = Particles.Where(p => p.Type == ParticleEnum.Peg).ToList();
        var balls = Particles.Where(p => p.Type == ParticleEnum.Ball).ToList();
        var pegsCount = pegs.Count;
        var ballsCount = balls.Count;

        for (var i = 0; i < ballsCount; i++)
        {
            var particleA = balls[i];
            if (particleA.Config.IsInactive) continue;

            for (var j = 0; j < pegsCount; j++)
            {
                var peg = pegs[j];
                if (peg.Config.IsInactive) continue;

                var collision = ParticleCollider.Check(particleA, peg);
                if (collision is null) continue;

                ParticleCollider.Resolve(collision, particleA, peg);
                OnParticleCollision(particleA, peg);
            }

            if (!Configs.IsCollisionActive) continue;
            for (var j = i + 1; j < ballsCount; j++)
            {
                var ball = balls[j];
                if (ball.Config.IsInactive) continue;

                var collision = ParticleCollider.Check(particleA, ball);
                if (collision is null) continue;

                ParticleCollider.Resolve(collision, particleA, ball);
                OnParticleCollision(particleA, ball);
            }
        }
    }

    private void ValidateFinished()
    {
        var particlesInactive = Particles.Count(p => p.Config.IsInactive);

        var maxStepsIsZero = MaxSteps == 0;
        var finishedByMaxSteps = CurrentStep >= MaxSteps && !maxStepsIsZero;
        // var finishedByInactiveBalls = particlesInactive >= BallsCount;
        var finishedByInactiveBalls = false;

        if (finishedByMaxSteps || finishedByInactiveBalls)
        {
            Configs.IsPaused = true;
            OnFinished();
        }
    }


    private void OnBorderCollision(Particle particle, BorderEnum borderEnum)
    {
        BorderCollision?.Invoke(this, new BorderCollisionEventArgs(particle, borderEnum));
    }

    private void OnParticleCollision(Particle particleA, Particle particleB)
    {
        ParticleCollision?.Invoke(this, new ParticleCollisionEventArgs(particleA, particleB));
    }

    private void OnFinished()
    {
        Finished?.Invoke(this, new FinishedEventArgs(CurrentTime));
    }

    private void OnStep()
    {
        Step?.Invoke(this, new StepEventArgs(CurrentStep, (float)CurrentTime));
    }
}