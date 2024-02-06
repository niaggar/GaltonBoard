using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using GaltonBoard.Core.Managers;
using GaltonBoard.Model.Configs;
using GaltonBoard.Model.Models;

namespace GaltonBoard.App.Windows;

public partial class ExecutionWindow : Window, INotifyPropertyChanged
{
    private ExperimentConfig Config { get; set; }
    private SimulationManager Manager { get; set; }
    private Stopwatch ExecutionTimer { get; set; }

    private ObservableCollection<SimulationExecutionStatus> simulations { get; set; }
    public ObservableCollection<SimulationExecutionStatus> Simulations
    {
        get => simulations;
        set
        {
            simulations = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public ExecutionWindow(ExperimentConfig config)
    {
        InitializeComponent();

        Config = config;
        Simulations = new ObservableCollection<SimulationExecutionStatus>();
        Manager = new SimulationManager(Config);

        Manager.SimulationStarted += SimulationStarted;
        Manager.SimulationFinished += SimulationFinished;
        Manager.AllSimulationsFinished += AllSimulationsFinished;
        Manager.SimulationStepFinished += SimulationStepFinished;
    }

    private void StartSimulation(object sender, RoutedEventArgs e)
    {
        ExecutionTimer = new Stopwatch();
        ExecutionTimer.Start();

        Manager.CreateBoards();
        Manager.StartExecution();
    }

    private async void SimulationStarted(object? sender, SimulationExecutionStatus simulation)
    {
        await Dispatcher.InvokeAsync(() => Simulations.Add(simulation));
    }

    private async void SimulationFinished(object? sender, SimulationExecutionStatus simulation)
    {
        await Dispatcher.InvokeAsync(() =>
        {
            var simulationToUpdate = Simulations.FirstOrDefault(s => s.ExecutionName == simulation.ExecutionName);
            if (simulationToUpdate == null)
            {
                Simulations.Add(simulation);
            }

            var index = Simulations.IndexOf(simulationToUpdate);
            Simulations[index] = simulation;
        });
    }

    private async void AllSimulationsFinished()
    {
        await Dispatcher.InvokeAsync(() =>
        {
            var executionTime = ExecutionTimer.Elapsed;
            MessageBox.Show($"All simulations finished in {executionTime.TotalSeconds} seconds");

            Close();
        });
    }

    private async void SimulationStepFinished(object? sender, SimulationExecutionStatus simulation)
    {
        await Dispatcher.InvokeAsync(() =>
        {
            var simulationToUpdate = Simulations.FirstOrDefault(s => s.ExecutionName == simulation.ExecutionName);
            if (simulationToUpdate == null)
            {
                Simulations.Add(simulation);
            }

            var index = Simulations.IndexOf(simulationToUpdate);
            Simulations[index] = simulation;
        });
    }

    private void ExecutionWindow_OnClosed(object? sender, EventArgs e)
    {
        Manager.CancelExecution();
    }
}