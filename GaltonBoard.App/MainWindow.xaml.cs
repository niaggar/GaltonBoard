using System.IO;
using System.Windows;
using GaltonBoard.App.Windows;
using GaltonBoard.Core.Utils;
using GaltonBoard.Model.Configs;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace GaltonBoard.App;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public ExperimentConfig ExperimentConfig { get; set; } = ExperimentConfig.Default;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void OpenCreationBallsConfig(object sender, RoutedEventArgs e)
    {
        var creationBallsConfigWindow = new CreationBallsConfigWindow(ExperimentConfig.BallCreationConfig) { Owner = this };
        creationBallsConfigWindow.ShowDialog();

        if (!creationBallsConfigWindow.DialogResult.HasValue) return;
        ShowValueCreationBallsConfig.Content = creationBallsConfigWindow.DialogResult == true ? "Modify" : "Default";
        ExperimentConfig.BallCreationConfig = creationBallsConfigWindow.Config;
    }

    private void OpenCreationPegsConfig(object sender, RoutedEventArgs e)
    {
        var creationPegsConfigWindow = new CreationPegsConfigWindow(ExperimentConfig.PegCreationConfig) { Owner = this };
        creationPegsConfigWindow.ShowDialog();

        if (!creationPegsConfigWindow.DialogResult.HasValue) return;
        ShowValueCreationPegsConfig.Content = creationPegsConfigWindow.DialogResult == true ? "Modify" : "Default";
        ExperimentConfig.PegCreationConfig = creationPegsConfigWindow.Config;
    }

    private void OpenExportConfig(object sender, RoutedEventArgs e)
    {
        var exportConfigWindow = new ExportConfigWindow(ExperimentConfig.ExportConfig) { Owner = this };
        exportConfigWindow.ShowDialog();

        if (!exportConfigWindow.DialogResult.HasValue) return;
        ShowValueExportConfig.Content = exportConfigWindow.DialogResult == true ? "Modify" : "Default";
        ExperimentConfig.ExportConfig = exportConfigWindow.Config;
    }

    private void OpenEngineConfig(object sender, RoutedEventArgs e)
    {
        var engineConfigWindow = new EngineConfigWindow(ExperimentConfig.EngineConfig, ExperimentConfig.TimeConfig) { Owner = this };
        engineConfigWindow.ShowDialog();

        if (!engineConfigWindow.DialogResult.HasValue) return;
        ShowValueEngineConfig.Content = engineConfigWindow.DialogResult == true ? "Modify" : "Default";
        ExperimentConfig.EngineConfig = engineConfigWindow.Config;
        ExperimentConfig.TimeConfig = engineConfigWindow.TimeConfig;
    }

    private void OpenBoardConfig(object sender, RoutedEventArgs e)
    {
        var boardConfigWindow = new BoardConfigWindow(ExperimentConfig.BoardConfig) { Owner = this };
        boardConfigWindow.ShowDialog();

        if (!boardConfigWindow.DialogResult.HasValue) return;
        ShowValueBoardConfig.Content = boardConfigWindow.DialogResult == true ? "Modify" : "Default";
        ExperimentConfig.BoardConfig = boardConfigWindow.Config;
    }

    private void StartExecution(object sender, RoutedEventArgs e)
    {
        var numberOfExecutions = int.Parse(NumberOfExecutions.Value);
        var experimentName = ExperimentName.Value;
        var numberSimultaneousExecution = int.Parse(NumberOfSimultaneousExecutions.Value);

        ExperimentConfig.NumberOfExecutions = numberOfExecutions;
        ExperimentConfig.Name = experimentName;
        ExperimentConfig.NumberOfSimultaneousExecutions = numberSimultaneousExecution;

        var executionWindow = new ExecutionWindow(ExperimentConfig) { Owner = this };
        executionWindow.ShowDialog();
    }

    private void StartTestExecution(object sender, RoutedEventArgs e)
    {
        var numberOfExecutions = int.Parse(NumberOfExecutions.Value);
        var experimentName = ExperimentName.Value;
        var numberSimultaneousExecution = int.Parse(NumberOfSimultaneousExecutions.Value);

        ExperimentConfig.NumberOfExecutions = numberOfExecutions;
        ExperimentConfig.Name = experimentName;
        ExperimentConfig.NumberOfSimultaneousExecutions = numberSimultaneousExecution;

        var simulationWindow = new SimulationWindow(ExperimentConfig) { Owner = this };
        simulationWindow.ShowDialog();
    }

    private void ExportConfig(object sender, RoutedEventArgs e)
    {
        var numberOfExecutions = int.Parse(NumberOfExecutions.Value);
        var experimentName = ExperimentName.Value;
        var numberSimultaneousExecution = int.Parse(NumberOfSimultaneousExecutions.Value);

        var configurationToExport = ExperimentConfig.DeepCopy();
        configurationToExport.NumberOfExecutions = numberOfExecutions;
        configurationToExport.Name = experimentName;
        configurationToExport.NumberOfSimultaneousExecutions = numberSimultaneousExecution;

        var saveFileDialog = new SaveFileDialog
        {
            Filter = "JSON file (*.json)|*.json",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            FileName = $"{configurationToExport.Name}.config.json"
        };

        if (saveFileDialog.ShowDialog() != true) return;
        var path = saveFileDialog.FileName;

        var json = JsonConvert.SerializeObject(configurationToExport, Formatting.Indented);
        File.WriteAllText(path, json);

        MessageBox.Show("Configuration exported successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

        var argument = "/select, \"" + path + "\"";
        System.Diagnostics.Process.Start("explorer.exe", argument);
    }

    private void LoadConfig(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "JSON file (*.json)|*.json",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
        };

        if (openFileDialog.ShowDialog() != true) return;
        var path = openFileDialog.FileName;

        var json = File.ReadAllText(path);
        var configurationToLoad = JsonConvert.DeserializeObject<ExperimentConfig>(json);
        if (configurationToLoad == null)
        {
            MessageBox.Show("Configuration could not be loaded!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        ExperimentConfig = configurationToLoad;
        NumberOfExecutions.SetValue(configurationToLoad.NumberOfExecutions.ToString());
        ExperimentName.SetValue(configurationToLoad.Name);
        NumberOfSimultaneousExecutions.SetValue(configurationToLoad.NumberOfSimultaneousExecutions.ToString());

        MessageBox.Show("Configuration loaded successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}