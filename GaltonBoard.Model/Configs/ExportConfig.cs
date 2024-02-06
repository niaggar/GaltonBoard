namespace GaltonBoard.Model.Configs;

public class ExportConfig
{
    public int StepsToExport { get; set; }
    public string Path { get; set; }
    public bool ExportPath { get; set; }
    public bool ExportHistogram { get; set; }
    public double PercentExperimentsToExport { get; set; }

    public string ExecutionName { get; set; }
    public string SaveExportPath => $"{Path}\\{ExecutionName}_path.dat";
    public string SaveExportHistogram => $"{Path}\\{ExecutionName}_histogram.dat";

    public static ExportConfig Default => new()
    {
        StepsToExport = 1,
        Path = "D:\\Tests",
        ExportPath = true,
        ExportHistogram = true,
        ExecutionName = "Default",
        PercentExperimentsToExport = 0.5
    };
}