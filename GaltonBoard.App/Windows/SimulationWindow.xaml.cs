using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Media.Imaging;
using GaltonBoard.Core;
using GaltonBoard.Core.Managers;
using GaltonBoard.Model.Configs;
using GaltonBoard.Model.Models;

namespace GaltonBoard.App.Windows;

public partial class SimulationWindow : Window
{
    private Bitmap _bmpLive;
    private Bitmap _bmpLast;
    private Border _imageSize;
    private CancellationTokenSource _cancellationTokenSource;
    private ExperimentConfig _config;

    private static readonly int _maxFPS = 60;
    private static readonly double _minFramePeriodSec = 1.0 / _maxFPS;

    private BlockingCollection<RenderParticle[]> _simulationQueue;
    private SimulationRenderManager _renderManager;

    public SimulationWindow(ExperimentConfig config)
    {
        InitializeComponent();

        _config = config;
        _cancellationTokenSource = new CancellationTokenSource();
        _simulationQueue = new BlockingCollection<RenderParticle[]>();
    }

    private void StartSimulationThread(CancellationToken cancellationToken)
    {
        _renderManager = new SimulationRenderManager(_config, _imageSize);
        _renderManager.Start();

        Task.Run(() =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var simulationData = SimulateAsync(cancellationToken);
                _simulationQueue.Add(simulationData, cancellationToken);
                Thread.Sleep((int)_minFramePeriodSec * 1000);
            }
        }, cancellationToken);
    }

    private void StartRenderThread(CancellationToken cancellationToken)
    {
        Task.Run(() =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (_renderManager.IsFinished())
                {
                    _renderManager.SaveAll();
                    _cancellationTokenSource?.Cancel();
                    return;
                }

                var simulationData = _simulationQueue.Take(cancellationToken);
                _renderManager.Render(simulationData, ref _bmpLive);

                lock (_bmpLast)
                {
                    _bmpLast.Dispose();
                    _bmpLast = (Bitmap)_bmpLive.Clone();
                    Dispatcher.Invoke(() => ImageRender.Source = BmpImageFromBmp(_bmpLast));
                }

                Thread.Sleep((int)_minFramePeriodSec * 1000);
            }

            _renderManager.SaveAll();
        }, cancellationToken);
    }

    private RenderParticle[] SimulateAsync(CancellationToken cancellationToken)
    {
        var simulationData = _renderManager.GetRenderParticles();
        _renderManager.RunStep(_minFramePeriodSec);

        return simulationData;
    }

    private BitmapImage BmpImageFromBmp(Image bmp)
    {
        using var memory = new System.IO.MemoryStream();

        bmp.Save(memory, ImageFormat.Png);
        memory.Position = 0;

        var bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.StreamSource = memory;
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.EndInit();
        bitmapImage.Freeze();

        return bitmapImage;
    }

    private void btnStart_Click(object sender, RoutedEventArgs e)
    {
        _imageSize = new Border
        {
            Width = CanvasRender.ActualWidth,
            Height = CanvasRender.ActualHeight
        };

        _bmpLive = new Bitmap((int)_imageSize.Width, (int)_imageSize.Height);
        _bmpLast = (Bitmap)_bmpLive.Clone();

        _cancellationTokenSource = new CancellationTokenSource();

        StartSimulationThread(_cancellationTokenSource.Token);
        StartRenderThread(_cancellationTokenSource.Token);
    }

    private void btnStop_Click(object sender, RoutedEventArgs e)
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = null;
    }
}