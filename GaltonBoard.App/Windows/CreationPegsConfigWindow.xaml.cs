using System.Globalization;
using System.Windows;
using GaltonBoard.Model.Configs;
using GaltonBoard.Model.Enums;
using GaltonBoard.Model.Models;

namespace GaltonBoard.App.Windows;

public partial class CreationPegsConfigWindow : Window
{
    public PegCreationConfig Config { get; set; }

    public CreationPegsConfigWindow(PegCreationConfig? config = null)
    {
        InitializeComponent();
        Config = config ?? PegCreationConfig.Default;
        SetValues(Config);
    }

    private void SetValues(PegCreationConfig config)
    {
        RestitutionInput.Value = config.Restitution.ToString(CultureInfo.InvariantCulture);
        RadiusMinInput.Value = config.Radio.Min.ToString(CultureInfo.InvariantCulture);
        RadiusMaxInput.Value = config.Radio.Max.ToString(CultureInfo.InvariantCulture);
        MassMinInput.Value = config.Mass.Min.ToString(CultureInfo.InvariantCulture);
        MassMaxInput.Value = config.Mass.Max.ToString(CultureInfo.InvariantCulture);
        AmplitudeXInput.Value = config.XAmplitude.ToString(CultureInfo.InvariantCulture);
        FrequencyXInput.Value = config.XFrequency.ToString(CultureInfo.InvariantCulture);
        AmplitudeYInput.Value = config.YAmplitude.ToString(CultureInfo.InvariantCulture);
        FrequencyYInput.Value = config.YFrequency.ToString(CultureInfo.InvariantCulture);
        DirectionCreateInput.Text = config.Direction switch
        {
            DirectionEnum.None => "None",
            DirectionEnum.Vertical => "Vertical",
            DirectionEnum.Horizontal => "Horizontal",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private void Save(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Config.Restitution = double.Parse(RestitutionInput.Value);
        Config.Radio = Range<double>.CreateMinMax(double.Parse(RadiusMinInput.Value), double.Parse(RadiusMaxInput.Value));
        Config.Mass = Range<double>.CreateMinMax(double.Parse(MassMinInput.Value), double.Parse(MassMaxInput.Value));
        Config.XAmplitude = float.Parse(AmplitudeXInput.Value);
        Config.XFrequency = float.Parse(FrequencyXInput.Value);
        Config.YAmplitude = float.Parse(AmplitudeYInput.Value);
        Config.YFrequency = float.Parse(FrequencyYInput.Value);
        Config.IsStatic = false;
        Config.Direction = DirectionCreateInput.Text switch
        {
            "None" => DirectionEnum.None,
            "Vertical" => DirectionEnum.Vertical,
            "Horizontal" => DirectionEnum.Horizontal,
            _ => throw new ArgumentOutOfRangeException()
        };

        Close();
    }

    private void Reset(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Config = PegCreationConfig.Default;

        Close();
    }
}