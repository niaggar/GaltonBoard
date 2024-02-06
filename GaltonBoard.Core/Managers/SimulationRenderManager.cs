using System.Drawing;
using System.Drawing.Drawing2D;
using GaltonBoard.Core.Logic;
using GaltonBoard.Model.Configs;
using GaltonBoard.Model.Models;

namespace GaltonBoard.Core.Managers;

public class SimulationRenderManager
{
    private GaltonBoardSimulation _simulation;
    private readonly ExperimentConfig _config;
    private Border _imageSize;

    public SimulationRenderManager(ExperimentConfig config, Border imageSize)
    {
        _config = config;
        _imageSize = ConfigureImageSize(imageSize);
    }

    public void Start()
    {
        _simulation = new GaltonBoardSimulation(_config);
        _simulation.Create();
    }

    public void RunStep(double time)
    {
        _simulation.RunStep(time);
    }

    public bool IsFinished()
    {
        return _simulation.IsPaused;
    }

    public RenderParticle[] GetRenderParticles()
    {
        var particles = _simulation.GetParticles();
        var renderParticles = new RenderParticle[particles.Length];

        for (var i = 0; i < particles.Length; i++)
        {
            renderParticles[i] = new RenderParticle(particles[i], _imageSize, _simulation.EngineConfig.Border);
        }

        return renderParticles;
    }

    public void Render(RenderParticle[] simulationData, ref Bitmap bmpLive)
    {
        var particleColor = Color.DodgerBlue;

        using var brush = new SolidBrush(particleColor);
        using var gfx = Graphics.FromImage(bmpLive);

        gfx.SmoothingMode = SmoothingMode.AntiAlias;
        gfx.Clear(Color.Black);

        var imageSize = _imageSize;
        var border = new RectangleF((float)imageSize.MarginWidth, (float)imageSize.MarginHeight, (float)imageSize.Width, (float)imageSize.Height);
        gfx.DrawRectangle(new Pen(Color.Gray), border);

        foreach (var particle in simulationData)
        {
            var xPixel = (float)particle.PositionX;
            var yPixel = (float)particle.PositionY;

            var radius = (float)particle.Radius;
            var diameter = radius * 2;

            gfx.FillEllipse(brush, xPixel - radius, yPixel - radius, diameter, diameter);
        }
    }

    public void SaveAll()
    {
        _simulation.ExportSaveAll();
    }

    public Border ConfigureImageSize(Border imageSize)
    {
        var engineHeight = _config.EngineConfig.Border.Height;
        var engineWidth = _config.EngineConfig.Border.Width;

        var ratioEngineImageHeight = imageSize.Height / engineHeight;
        var newEngineWidth = engineWidth * ratioEngineImageHeight;
        var margin = (imageSize.Width - newEngineWidth) / 2;

        return new Border
        {
            Height = imageSize.Height,
            Width = newEngineWidth,
            MarginHeight = 0,
            MarginWidth = margin,
            Restitution = _config.EngineConfig.Border.Restitution
        };
    }
}