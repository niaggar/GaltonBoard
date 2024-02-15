using GaltonBoard.Core.Events;
using GaltonBoard.Model.Configs;
using GaltonBoard.Model.Enums;
using GaltonBoard.Model.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GaltonBoard.Core.Logic;

public class Engine(EngineConfig configs, BoardConfig boardConfig, int particlesCount, int pegsCount)
{
    public double CurrentTime { get; set; } = 0.0d;
    public int CurrentStep { get; set; } = 0;
    public int MaxSteps { get; set; } = 0;

    public int BallsCount { get; set; } = particlesCount;
    public int PegsCount { get; set; } = pegsCount;


    public EngineConfig Configs { get; set; } = configs;
    public BoardConfig BoardConfig { get; set; } = boardConfig;
    public List<Particle> Particles { get; set; } = new() { Capacity = particlesCount + pegsCount };


    public event EventHandler<BorderCollisionEventArgs>? BorderCollision;
    public event EventHandler<ParticleCollisionEventArgs>? ParticleCollision;
    public event EventHandler<FinishedEventArgs>? Finished;
    public event EventHandler<StepEventArgs>? Step;

    public Grid BoardGrid { get; set; } = new(configs.Border);


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
            UpdateParticles(deltaSubStep);

            if (Constants.IsParallel)
            {
                UpdateGrid();
                ValidateCollisionsParallel();
            }
            else
            {
                ValidateCollisions();
            }
        }

        CurrentTime += deltaTime;
        CurrentStep++;

        OnStep();
        ValidateFinished();
    }

    private void UpdateGrid()
    {
        BoardGrid.Clear();
        foreach (var particle in Particles)
        {
            BoardGrid.AddParticle(particle);
        }
    }

    private void ValidateCollisionsParallel()
    {
        var rows = BoardGrid.Rows;
        var columns = BoardGrid.Columns;

        var sectorRows = rows / Constants.NumberOfSimultaneousCollisionsValidation;
        var sectorColumns = columns / Constants.NumberOfSimultaneousCollisionsValidation;

        if (sectorRows == 0) sectorRows = 1;
        if (sectorColumns == 0) sectorColumns = 1;


        for (var i = 0; i < sectorRows; i++)
        {
            for (var j = 0; j < sectorColumns; j++)
            {
                var row = i * sectorRows;
                var column = j * sectorColumns;

                var finalRow = row + sectorRows;
                var finalColumn = column + sectorColumns;

                ValidateCollisionsSector(row, column, finalRow, finalColumn);
            }
        }
    }

    private void ValidateCollisionsSector(int initialRow, int initialColumn, int finalRow, int finalColumn)
    {
        for (var internalRow = initialRow; internalRow < finalRow; internalRow++)
        {
            for (var internalColumn = initialColumn; internalColumn < finalColumn; internalColumn++)
            {
                var cells = BoardGrid.GetCellAndNeighbors(internalRow, internalColumn);

                var pegs = cells.SelectMany(c => c.Pegs).ToList();
                var balls = cells.SelectMany(c => c.Balls).ToList();

                for (var i = 0; i < balls.Count; i++)
                {
                    var particleA = balls[i];
                    if (particleA.Config.IsInactive) continue;

                    for (var iPeg = 0; iPeg < pegs.Count; iPeg++)
                    {
                        var peg = pegs[iPeg];
                        if (peg.Config.IsInactive) continue;

                        var collision = ParticleCollider.Check(particleA, peg);
                        if (collision is null) continue;

                        ParticleCollider.Resolve(collision, particleA, peg);
                        OnParticleCollision(particleA, peg);
                    }

                    if (!Configs.IsCollisionActive) continue;
                    for (var j = i + 1; j < balls.Count; j++)
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
        }
    }


    private void UpdateParticles(float deltaTime)
    {
        var particlesCount = Particles.Count;
        Parallel.For(0, particlesCount, (i, state) =>
        {
            var particle = Particles[i];
            if (particle.Config.IsInactive) return;

            particle.ApplyDrag((float)Configs.Drag);
            particle.ApplyForce(Configs.Gravity);
            particle.Update(deltaTime);

            var borderCollision = BorderCollider.Check(Configs.Border, particle.Position);
            if (borderCollision == BorderEnum.None) return;

            BorderCollider.Resolve(Configs.Border, borderCollision, particle);
            OnBorderCollision(particle, borderCollision);
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
        var finishedByInactiveBalls = particlesInactive >= BallsCount;

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
        var particles = Particles.ToArray();
        Step?.Invoke(this, new StepEventArgs(CurrentStep, (float)CurrentTime, particles));
    }
}