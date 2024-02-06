using GaltonBoard.Model.Enums;
using GaltonBoard.Model.Models;

namespace GaltonBoard.Model.Configs;

public class ExperimentConfig
{
    public string Name { get; set; }
    public int NumberOfExecutions { get; set; }
    public int NumberOfSimultaneousExecutions { get; set; }
    public EngineConfig EngineConfig { get; set; }
    public BallCreationConfig BallCreationConfig { get; set; }
    public PegCreationConfig PegCreationConfig { get; set; }
    public ExportConfig ExportConfig { get; set; }
    public TimeConfig TimeConfig { get; set; }
    public BoardConfig BoardConfig { get; set; }

    public static ExperimentConfig Default => new()
    {
        Name = "Default Experiment",
        NumberOfExecutions = 1,
        NumberOfSimultaneousExecutions = 5,
        EngineConfig = EngineConfig.Default,
        BallCreationConfig = BallCreationConfig.Default,
        PegCreationConfig = PegCreationConfig.Default,
        ExportConfig = ExportConfig.Default,
        TimeConfig = TimeConfig.Default,
        BoardConfig = BoardConfig.Default,
    };
}