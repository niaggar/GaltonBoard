using System.Globalization;
using System.Windows;
using GaltonBoard.Model.Configs;
using GaltonBoard.Model.Models;

namespace GaltonBoard.App.Windows;

public partial class EngineConfigWindow : Window
{
    public EngineConfig Config { get; set; }
    public TimeConfig TimeConfig { get; set; }

    public EngineConfigWindow(EngineConfig? config = null, TimeConfig? timeConfig = null)
    {
        InitializeComponent();
        Config = config ?? EngineConfig.Default;
        TimeConfig = timeConfig ?? TimeConfig.Default;
        SetValues(Config, TimeConfig);
    }

    private void SetValues(EngineConfig config, TimeConfig timeConfig)
    {
        GravityXValueInput.Value = config.Gravity.X.ToString(CultureInfo.InvariantCulture);
        GravityYValueInput.Value = config.Gravity.Y.ToString(CultureInfo.InvariantCulture);
        HeightValueInput.Value = config.Border.Height.ToString(CultureInfo.InvariantCulture);
        WidthValueInput.Value = config.Border.Width.ToString(CultureInfo.InvariantCulture);
        RestitutionValueInput.Value = config.Border.Restitution.ToString(CultureInfo.InvariantCulture);
        DragValueInput.Value = config.Drag.ToString(CultureInfo.InvariantCulture);
        CollisionActiveInput.IsChecked = config.IsCollisionActive;

        StepValueInput.Value = timeConfig.TimeStep.ToString(CultureInfo.InvariantCulture);
        SubstepsValueInput.Value = timeConfig.SubSteps.ToString();
        MaxStepsValueInput.Value = timeConfig.MaxSteps.ToString();
    }

    private void Save(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Config.Gravity = new GaltonBoard.Model.Models.Vector(double.Parse(GravityXValueInput.Value), double.Parse(GravityYValueInput.Value));
        Config.Border = new Border() { Height = double.Parse(HeightValueInput.Value), Width = double.Parse(WidthValueInput.Value), Restitution = double.Parse(RestitutionValueInput.Value) };
        Config.Drag = double.Parse(DragValueInput.Value);
        Config.IsCollisionActive = CollisionActiveInput.IsChecked ?? false;

        TimeConfig.TimeStep = double.Parse(StepValueInput.Value);
        TimeConfig.SubSteps = int.Parse(SubstepsValueInput.Value);
        TimeConfig.MaxSteps = int.Parse(MaxStepsValueInput.Value);

        Close();
    }

    private void Reset(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Config = EngineConfig.Default;
        TimeConfig = TimeConfig.Default;

        Close();
    }
}