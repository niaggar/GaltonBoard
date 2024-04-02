using System.Globalization;
using GaltonBoard.Core.Utils;
using GaltonBoard.Model.Configs;
using GaltonBoard.Model.Enums;
using GaltonBoard.Model.Models;

namespace GaltonBoard.Core.Logic;

public class Exporter
{
    private ExportConfig ExportConfig { get; set; }

    private MemoryStream PathStream { get; set; }
    private StreamWriter PathWriter { get; set; }

    private MemoryStream HistogramStream { get; set; }
    private StreamWriter HistogramWriter { get; set; }

    private Vector[]? DrawBorder { get; set; }


    public Exporter(ExportConfig exportConfig, Vector[]? drawBorder = null)
    {
        ExportConfig = exportConfig;
        DrawBorder = drawBorder;

        if (ExportConfig.ExportPath)
        {
            PathStream = new MemoryStream();
            PathWriter = new StreamWriter(PathStream);
        }

        if (ExportConfig.ExportHistogram)
        {
            HistogramStream = new MemoryStream();
            HistogramWriter = new StreamWriter(HistogramStream);
        }
    }

    public void ExportStep(Particle[] particles, Border border)
    {
        if (!ExportConfig.ExportPath) return;

        var totalNumberOfParticles = particles.Length;
        var totalNumberOfDrawBorderPoints = DrawBorder?.Length ?? 0;

        var totalOfPoints = totalNumberOfParticles + totalNumberOfDrawBorderPoints;
        var headerString = GetExportHeaderString(totalOfPoints);

        PathWriter.Write(headerString);
        for (var i = 0; i < totalNumberOfParticles; i++)
        {
            var particle = particles[i];
            var particleString = GetExportParticleString(particle, i);

            PathWriter.Write(particleString);
        }

        var borderString = GetExportBorderString(DrawBorder, totalNumberOfParticles);
        PathWriter.Write(borderString);
        PathWriter.Flush();
    }

    public void ExportHistogram(Histogram histogram)
    {
        if (!ExportConfig.ExportHistogram) return;

        var histogramString = GetExportHistogramString(histogram);

        HistogramWriter.Write(histogramString);
        HistogramWriter.Flush();
    }

    public void Save()
    {
        if (ExportConfig.ExportPath)
        {
            var path = ExportConfig.SaveExportPath;
            using var fileStream = new FileStream(path, FileMode.Create);

            PathStream.Position = 0;
            PathStream.WriteTo(fileStream);
            PathWriter.Dispose();
            PathStream.Dispose();
        }

        if (ExportConfig.ExportHistogram)
        {
            var histogram = ExportConfig.SaveExportHistogram;
            using var fileStream = new FileStream(histogram, FileMode.Create);

            HistogramStream.Position = 0;
            HistogramStream.WriteTo(fileStream);
            HistogramWriter.Dispose();
            HistogramStream.Dispose();
        }
    }

    private static string GetExportHeaderString(int totalNumberOfParticles)
    {
        return $"{totalNumberOfParticles}\naver\n";
    }

    private static string GetExportParticleString(Particle particle, int num)
    {
        var position = particle.Position;
        var type = (int)particle.Type;
        var radius = particle.Config.Radius;

        return $"{num}\t{type}\t{position.X.ToString(CultureInfo.InvariantCulture)}\t{position.Y.ToString(CultureInfo.InvariantCulture)}\t{radius.ToString(CultureInfo.InvariantCulture)}\n";
    }

    private static string GetExportBorderString(Vector[]? drawBorder, int lastParticleIndex)
    {
        if (drawBorder == null) return string.Empty;
        return drawBorder
            .Select((point, i) => $"{lastParticleIndex + i}\t{2}\t{point.X.ToString(CultureInfo.InvariantCulture)}\t{point.Y.ToString(CultureInfo.InvariantCulture)}\t{Constants.RadiusBorderScale.ToString(CultureInfo.InvariantCulture)}\n")
            .Aggregate(string.Empty, (current, pointString) => current + pointString);
    }

    private static string GetExportHistogramString(Histogram histogram)
    {
        var histogramString = "Bin\tCount\n";
        var bins = histogram.Bins;
        var totalBins = bins.Length;

        for (var i = 0; i < totalBins; i++)
        {
            var bin = bins[i];
            var binString = $"{i}\t{bin}\n";
            histogramString += binString;
        }

        return histogramString;
    }
}