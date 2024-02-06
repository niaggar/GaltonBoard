using GaltonBoard.Core.Logic;
using GaltonBoard.Core.Utils;
using GaltonBoard.Model.Configs;
using GaltonBoard.Model.Enums;
using GaltonBoard.Model.Models;

namespace GaltonBoard.Core.Managers;

public class SimulationManager
{
    public event EventHandler<SimulationExecutionStatus> SimulationFinished;
    public event EventHandler<SimulationExecutionStatus> SimulationStarted;
    public event EventHandler<SimulationExecutionStatus> SimulationStepFinished;
    public event Action AllSimulationsFinished;

    private readonly ExperimentConfig _config;
    private readonly List<GaltonBoardSimulation> _boards;
    private readonly List<Task<SimulationExecutionStatus>> _simulations;
    private readonly List<CancellationTokenSource> _cancellationTokenSources;

    private SemaphoreSlim _semaphore;
    private int _finishedSimulations;

    public SimulationManager(ExperimentConfig config)
    {
        _config = config;
        _simulations = new List<Task<SimulationExecutionStatus>>();
        _cancellationTokenSources = new List<CancellationTokenSource>();
        _boards = new List<GaltonBoardSimulation>();
        _semaphore = new SemaphoreSlim(_config.NumberOfSimultaneousExecutions);
    }

    public void CreateBoards()
    {
        var numberOfBoardsToExport = (int)(_config.NumberOfExecutions * _config.ExportConfig.PercentExperimentsToExport);
        if (numberOfBoardsToExport < 1) numberOfBoardsToExport = 1;

        for(var i = 0; i < _config.NumberOfExecutions; i++)
        {
            var executionName = $"{_config.Name} - Execution {i + 1}";
            var newConfig = _config.DeepCopy();

            var exportPath = _config.ExportConfig.ExportPath;
            newConfig.ExportConfig.ExportPath = exportPath && i < numberOfBoardsToExport;
            newConfig.ExportConfig.ExecutionName = executionName;

            var board = new GaltonBoardSimulation(newConfig);
            board.Create();

            _boards.Add(board);

            var executionStatus = board.GetExecution();
            executionStatus.State = ExecutionStateEnum.NotStarted;
            SimulationStarted?.Invoke(this, executionStatus);
        }
    }

    public void StartExecution()
    {
        _semaphore = new SemaphoreSlim(_config.NumberOfSimultaneousExecutions);
        _simulations.Clear();
        _cancellationTokenSources.Clear();

        foreach (var board in _boards)
        {
            StartSimulation(board);
        }
    }

    public void CancelExecution()
    {
        foreach (var cancellationTokenSource in _cancellationTokenSources)
        {
            cancellationTokenSource.Cancel();
        }
    }

    private void StartSimulation(GaltonBoardSimulation board)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        _cancellationTokenSources.Add(cancellationTokenSource);

        Console.WriteLine($"Simulation {board.ExportConfig.ExecutionName} Started");

        var task = new Task<SimulationExecutionStatus>(() => RunBoard(board, cancellationTokenSource.Token), cancellationTokenSource.Token);
        task.ContinueWith(t => OnFinishedSimulation(task.Id, t.Result), TaskContinuationOptions.OnlyOnRanToCompletion);
        task.ContinueWith(t => OnCanceledSimulation(task.Id, t.Result), TaskContinuationOptions.OnlyOnCanceled);
        task.ContinueWith(t => _semaphore.Release(), TaskContinuationOptions.None);
        task.Start();

        _simulations.Add(task);
    }

    private void OnFinishedSimulation(int id, SimulationExecutionStatus simulation)
    {
        Console.WriteLine($"Simulation {simulation.ExecutionName} finished");
        SimulationFinished?.Invoke(this, simulation);
        _finishedSimulations++;

        if (_finishedSimulations == _config.NumberOfExecutions)
        {
            OnAllSimulationsFinished();
        }
    }

    private void OnCanceledSimulation(int id, SimulationExecutionStatus simulation)
    {
        Console.WriteLine($"Simulation {simulation.ExecutionName} cancelled");
        simulation.State = ExecutionStateEnum.Cancelled;
        SimulationFinished?.Invoke(this, simulation);
        _finishedSimulations++;

        if (_finishedSimulations == _config.NumberOfExecutions)
        {
            AllSimulationsFinished?.Invoke();
        }
    }

    private void OnAllSimulationsFinished()
    {
        if (!_config.ExportConfig.ExportHistogram)
        {
            AllSimulationsFinished?.Invoke();
            return;
        }

        var histograms = _boards.Select(b => b.Histogram).ToArray();
        var histogramMean = Histogram.CreateMean(histograms!, true);

        var exportConfig = _config.ExportConfig.DeepCopy();
        exportConfig.ExecutionName = "MEAN";
        exportConfig.ExportPath = false;
        exportConfig.ExportHistogram = true;

        var exporter = new Exporter(exportConfig);
        exporter.ExportHistogram(histogramMean);
        exporter.Save();

        AllSimulationsFinished?.Invoke();
    }

    private SimulationExecutionStatus RunBoard(GaltonBoardSimulation board, CancellationToken token)
    {
        _semaphore.Wait(token);

        var deltaTime = board.TimeConfig.TimeStep;
        var maxSteps = board.TimeConfig.MaxSteps;
        var finished = false;

        board.StepsToNotify = Constants.StepsToNotify;
        board.Step += (sender, args) => SimulationStepFinished?.Invoke(this, args);
        board.Finished += (sender, args) => finished = true;

        for (var step = 0; step < maxSteps; step++)
        {
            if (finished) break;
            if (token.IsCancellationRequested)
            {
                var cancelledExecution = board.GetExecution();
                cancelledExecution.State = ExecutionStateEnum.Cancelled;
                return cancelledExecution;
            }

            if (board.IsPaused) break;
            board.RunStep(deltaTime);
        }

        board.ExportHistogram();
        board.ExportSaveAll();

        var finishedExecution = board.GetExecution();
        finishedExecution.State = ExecutionStateEnum.Finished;
        return finishedExecution;
    }
}