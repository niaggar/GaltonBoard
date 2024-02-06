using System.Globalization;
using System.Windows;
using GaltonBoard.Model.Configs;

namespace GaltonBoard.App.Windows;

public partial class ExportConfigWindow : Window
{
    public ExportConfig Config { get; set; }

    public ExportConfigWindow(ExportConfig? config = null)
    {
        InitializeComponent();
        Config = config ?? ExportConfig.Default;
        SetValues(Config);
    }

    private void SetValues(ExportConfig config)
    {
        StepsToExportInput.Value = config.StepsToExport.ToString();
        SaveRouteInput.Value = config.Path;
        SavePathInput.IsChecked = config.ExportPath;
        SaveHistogramInput.IsChecked = config.ExportHistogram;
        ExperimentsToExportInput.Value = config.PercentExperimentsToExport.ToString(CultureInfo.InvariantCulture);
    }

    private void Save(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Config.StepsToExport = int.Parse(StepsToExportInput.Value);
        Config.Path = SaveRouteInput.Value;
        Config.ExportPath = SavePathInput.IsChecked ?? false;
        Config.ExportHistogram = SaveHistogramInput.IsChecked ?? false;
        Config.PercentExperimentsToExport = double.Parse(ExperimentsToExportInput.Value, CultureInfo.InvariantCulture);

        Close();
    }

    private void Reset(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Config = ExportConfig.Default;

        Close();
    }
}