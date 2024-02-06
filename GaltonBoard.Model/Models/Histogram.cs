namespace GaltonBoard.Model.Models;

public class Histogram
{
    public int[] Bins { get; set; }
    public int TotalNumberOfParticles { get; set; }
    public int? TotalNumberOfExperiments { get; set; }

    public static Histogram Create(Particle[] particles, int numberOfBins, float width)
    {
        var histogram = new Histogram
        {
            Bins = new int[numberOfBins],
            TotalNumberOfParticles = particles.Length
        };

        foreach (var particle in particles)
        {
            var binIndex = (int) Math.Floor(particle.Position.X / width * numberOfBins);

            if (binIndex < 0) binIndex = 0;
            if (binIndex >= numberOfBins) binIndex = numberOfBins - 1;

            histogram.Bins[binIndex]++;
        }

        return histogram;
    }

    public static Histogram CreateMean(Histogram[] histograms, bool normalize = false)
    {
        var histogram = new Histogram
        {
            Bins = new int[histograms[0].Bins.Length],
            TotalNumberOfParticles = histograms[0].TotalNumberOfParticles,
            TotalNumberOfExperiments = histograms.Length
        };

        for (var i = 0; i < histogram.Bins.Length; i++)
        {
            histogram.Bins[i] = histograms.Sum(h => h.Bins[i]);
        }

        if (normalize)
        {
            var totalParticles = histogram.TotalNumberOfParticles * histogram.TotalNumberOfExperiments.Value;
            for (var i = 0; i < histogram.Bins.Length; i++)
            {
                histogram.Bins[i] /= totalParticles;
            }
        }

        return histogram;
    }
}