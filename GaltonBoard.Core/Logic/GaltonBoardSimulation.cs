using System.Diagnostics;
using GaltonBoard.Core.Utils;
using GaltonBoard.Model.Configs;
using GaltonBoard.Model.Enums;
using GaltonBoard.Model.Models;

namespace GaltonBoard.Core.Logic;

public class GaltonBoardSimulation
{
    public GaltonBoardSimulation(ExperimentConfig experimentConfig)
    {
        EngineConfig = experimentConfig.EngineConfig;
        BallCreationConfig = experimentConfig.BallCreationConfig;
        PegCreationConfig = experimentConfig.PegCreationConfig;
        ExportConfig = experimentConfig.ExportConfig;
        TimeConfig = experimentConfig.TimeConfig;
        BoardConfig = experimentConfig.BoardConfig;
    }

    public string Name => ExportConfig.ExecutionName;
    public EngineConfig EngineConfig { get; }
    public BallCreationConfig BallCreationConfig { get; }
    public PegCreationConfig PegCreationConfig { get; }
    public ExportConfig ExportConfig { get; }
    public TimeConfig TimeConfig { get; }
    public BoardConfig BoardConfig { get; set; }

    public Engine Engine { get; set; }
    public Exporter Exporter { get; set; }
    public Histogram? Histogram { get; set; }

    public bool IsPaused { get; set; } = false;
    public int CurrentStep { get; set; }
    public Stopwatch Timer { get; set; }

    public Particle[] Balls { get; set; }
    public Particle[] Pegs { get; set; }

    public int StepsToNotify;
    public event EventHandler<SimulationExecutionStatus> Step;
    public event EventHandler<SimulationExecutionStatus> Finished;

    public void RunStep(double step)
    {
        if (IsPaused) return;
        if (Timer.IsRunning == false) Timer.Start();

        CurrentStep++;
        Engine.RunStep((float)step, TimeConfig.SubSteps);
    }

    public void Create()
    {
        var pegs = ParticlesFactory.CreatePegs(PegCreationConfig, BoardConfig);
        var border = BorderFactory.CreateBorder(pegs, BoardConfig);
        var balls = ParticlesFactory.CreateBalls(BallCreationConfig, border);

        Balls = balls;
        Pegs = pegs;
        EngineConfig.Border = border;
        Engine = new Engine(EngineConfig, balls.Length, pegs.Length);
        Exporter = new Exporter(ExportConfig);
        Timer = new Stopwatch();

        Engine.MaxSteps = TimeConfig.MaxSteps;

        Engine.Finished += (sender, args) =>
        {
            IsPaused = true;
            Timer.Stop();

            var execution = GetExecution();
            execution.State = ExecutionStateEnum.Finished;
            Finished?.Invoke(this, execution);
        };

        Engine.BorderCollision += (sender, args) =>
        {
            if (args.Border != BorderEnum.Bottom) return;
            args.Particle.Config.IsInactive = true;
        };

        Engine.Step += (sender, args) =>
        {
            AddBall(args.CurrentStep);
            ExportPath(args.CurrentStep);
            if (CurrentStep % StepsToNotify != 0) return;

            var execution = GetExecution();
            execution.State = ExecutionStateEnum.Running;
            Step?.Invoke(this, execution);
        };

        foreach (var peg in pegs)
        {
            Engine.AddParticle(peg);
        }
    }

    public void ExportPath(int step)
    {
        var stepToExport = ExportConfig.StepsToExport;
        if (step % stepToExport != 0) return;

        var particles = Engine.Particles.ToArray();
        var border = Engine.Configs.Border;

        Exporter.ExportStep(particles, border);
    }

    public void ExportHistogram()
    {
        var balls = Engine.Particles.Where(p => p is Ball).ToArray();
        var border = Engine.Configs.Border;
        var numberOfBins = BoardConfig.NumberOfColumns;
        Histogram = Histogram.Create(balls, numberOfBins, (float)border.Width);

        Exporter.ExportHistogram(Histogram);
    }

    public void ExportSaveAll()
    {
        Exporter.Save();
    }
    
    public SimulationExecutionStatus GetExecution()
    {
        return new SimulationExecutionStatus
        {
            ExecutionName = ExportConfig.ExecutionName,
            MaxSteps = TimeConfig.MaxSteps,
            CurrentStep = CurrentStep,
            PathBallsRoute = ExportConfig.SaveExportPath,
            HistogramRoute = ExportConfig.SaveExportHistogram,
            TimeElapsed = Timer.Elapsed
        };
    }

    private void AddBall(int step)
    {
        var currentNumberOfBalls = Engine.Particles.Count(p => p is Ball);
        if (currentNumberOfBalls >= BallCreationConfig.NumberOfBalls) return;

        var stepToCreateBall =  BallCreationConfig.CreationStepInterval;
        if (step % stepToCreateBall != 0) return;

        var ball = Balls[currentNumberOfBalls];
        Engine.AddParticle(ball);
    }

    public Particle[] GetParticles()
    {
        return Engine.Particles.ToArray();
    }
}