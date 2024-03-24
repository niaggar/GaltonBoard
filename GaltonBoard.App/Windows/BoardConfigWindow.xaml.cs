using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using GaltonBoard.Model.Configs;
using GaltonBoard.Model.Enums;
using GaltonBoard.Model.Models;

namespace GaltonBoard.App.Windows;

public partial class BoardConfigWindow : Window
{
    public BoardConfig Config { get; set; }

    public BoardConfigWindow(BoardConfig? config = null)
    {
        InitializeComponent();
        Config = config ?? BoardConfig.Default;
        SetValues(Config);
    }

    private void SetValues(BoardConfig config)
    {
        TableColumnsInput.Value = config.NumberOfColumns.ToString(CultureInfo.InvariantCulture);
        TableRowsInput.Value = config.NumberOfRows.ToString(CultureInfo.InvariantCulture);
        MarginDownInput.Value = config.MarginDown.ToString(CultureInfo.InvariantCulture);
        MarginTopInput.Value = config.MarginUp.ToString(CultureInfo.InvariantCulture);
        MarginSidesInput.Value = config.MarginSides.ToString(CultureInfo.InvariantCulture);
        RestitutionInput.Value = config.Restitution.ToString(CultureInfo.InvariantCulture);
        FactXInput.Value = config.ResizeFactorX.ToString(CultureInfo.InvariantCulture);
        FactYInput.Value = config.ResizeFactorY.ToString(CultureInfo.InvariantCulture);
        RowsHeightInput.Value = config.RowsHeight.ToString(CultureInfo.InvariantCulture);
        ColumnsWidthInput.Value = config.ColumnsWidth.ToString(CultureInfo.InvariantCulture);

        var distributions = PegsDistributions.Children.OfType<RadioButton>().ToList();
        distributions.First(r => r.Content.ToString() == config.PegsDistribution.ToString()).IsChecked = true;
    }

    private void Save(object sender, RoutedEventArgs e)
    {
        var distributions = PegsDistributions.Children.OfType<RadioButton>().ToList();

        DialogResult = true;
        Config = new BoardConfig
        {
            NumberOfColumns = int.Parse(TableColumnsInput.Value),
            NumberOfRows = int.Parse(TableRowsInput.Value),
            MarginDown = double.Parse(MarginDownInput.Value),
            MarginUp = double.Parse(MarginTopInput.Value),
            MarginSides = double.Parse(MarginSidesInput.Value),
            Restitution = double.Parse(RestitutionInput.Value),
            ResizeFactorX = double.Parse(FactXInput.Value),
            ResizeFactorY = double.Parse(FactYInput.Value),
            RowsHeight = double.Parse(RowsHeightInput.Value),
            ColumnsWidth = double.Parse(ColumnsWidthInput.Value),
            PegsDistribution = distributions.First(r => r.IsChecked!.Value).Content.ToString() switch
            {
                "Default" => PegsDistributionEnum.Default,
                "Rectangular" => PegsDistributionEnum.Rectangular,
                _ => PegsDistributionEnum.Default,
            }
        };

        Close();
    }

    private void Reset(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Config = BoardConfig.Default;

        Close();
    }
}