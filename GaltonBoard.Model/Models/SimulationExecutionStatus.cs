using GaltonBoard.Model.Enums;

namespace GaltonBoard.Model.Models;

public class SimulationExecutionStatus : ObjectNotification
{
    private string executionName { get; set; }
    private int maxSteps { get; set; }
    private int currentStep { get; set; }
    private ExecutionStateEnum state { get; set; } = ExecutionStateEnum.NotStarted;
    private string? pathBallsRoute { get; set; }
    private string? histogramRoute { get; set; }
    private TimeSpan? timeElapsed { get; set; }
    private string stepsFinished => $"{CurrentStep}/{MaxSteps}";

    public string ExecutionName
    {
        get => executionName;
        set
        {
            executionName = value;
            OnPropertyChanged();
        }
    }

    public int MaxSteps
    {
        get => maxSteps;
        set
        {
            maxSteps = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(StepsFinished));
        }
    }

    public int CurrentStep
    {
        get => currentStep;
        set
        {
            currentStep = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(StepsFinished));
        }
    }

    public ExecutionStateEnum State
    {
        get => state;
        set
        {
            state = value;
            OnPropertyChanged();
        }
    }

    public string? PathBallsRoute
    {
        get => pathBallsRoute;
        set
        {
            pathBallsRoute = value;
            OnPropertyChanged();
        }
    }

    public string? HistogramRoute
    {
        get => histogramRoute;
        set
        {
            histogramRoute = value;
            OnPropertyChanged();
        }
    }

    public TimeSpan? TimeElapsed
    {
        get => timeElapsed;
        set
        {
            timeElapsed = value;
            OnPropertyChanged();
        }
    }

    public string StepsFinished => stepsFinished;
}