using System.Globalization;
using System.Windows;
using GaltonBoard.Model.Configs;
using GaltonBoard.Model.Models;

namespace GaltonBoard.App.Windows;

public partial class CreationBallsConfigWindow : Window
{
    public BallCreationConfig Config { get; set; }

    public CreationBallsConfigWindow(BallCreationConfig? config = null)
    {
        InitializeComponent();

        Config = config ?? BallCreationConfig.Default;
        SetValues(Config);
    }

    private void SetValues(BallCreationConfig config)
    {
        NumberOfBallsInput.Value = config.NumberOfBalls.ToString();
        CreationIntervalInput.Value = config.CreationStepInterval.ToString();
        RestitutionInput.Value = config.Restitution.ToString(CultureInfo.InvariantCulture);
        RadiosMinInput.Value = config.Radio.Min.ToString(CultureInfo.InvariantCulture);
        RadiosMaxInput.Value = config.Radio.Max.ToString(CultureInfo.InvariantCulture);
        MassMinInput.Value = config.Mass.Min.ToString(CultureInfo.InvariantCulture);
        MassMaxInput.Value = config.Mass.Max.ToString(CultureInfo.InvariantCulture);
        XOriginMinInput.Value = config.CenterX.Min.ToString(CultureInfo.InvariantCulture);
        XOriginMaxInput.Value = config.CenterX.Max.ToString(CultureInfo.InvariantCulture);
        YOriginMinInput.Value = config.CenterY.Min.ToString(CultureInfo.InvariantCulture);
        YOriginMaxInput.Value = config.CenterY.Max.ToString(CultureInfo.InvariantCulture);
        VelocityXMinAngleInput.Value = config.VelocityAngleRange.Min.ToString(CultureInfo.InvariantCulture);
        VelocityXMaxAngleInput.Value = config.VelocityAngleRange.Max.ToString(CultureInfo.InvariantCulture);
        VelocityMagnitudeInput.Value = config.VelocityMagnitude.ToString(CultureInfo.InvariantCulture);
    }

    private void Save(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Config.NumberOfBalls = int.Parse(NumberOfBallsInput.Value);
        Config.CreationStepInterval = int.Parse(CreationIntervalInput.Value);
        Config.Restitution = double.Parse(RestitutionInput.Value);
        Config.Radio = Range<double>.CreateMinMax(double.Parse(RadiosMinInput.Value), double.Parse(RadiosMaxInput.Value));
        Config.Mass = Range<double>.CreateMinMax(double.Parse(MassMinInput.Value), double.Parse(MassMaxInput.Value));
        Config.CenterX = Range<double>.CreateMinMax(double.Parse(XOriginMinInput.Value), double.Parse(XOriginMaxInput.Value));
        Config.CenterY = Range<double>.CreateMinMax(double.Parse(YOriginMinInput.Value), double.Parse(YOriginMaxInput.Value));
        Config.VelocityAngleRange = Range<double>.CreateMinMax(double.Parse(VelocityXMinAngleInput.Value), double.Parse(VelocityXMaxAngleInput.Value));
        Config.VelocityMagnitude = double.Parse(VelocityMagnitudeInput.Value);

        Close();
    }

    private void Reset(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Config = BallCreationConfig.Default;

        Close();
    }
}