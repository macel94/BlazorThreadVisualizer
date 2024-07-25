using BlazorThreadVisualizer.Models;

namespace BlazorThreadVisualizer.Services;

public class ThreadService
{
    private List<BlazorThread> _threads;
    private int _nextId = 0;
    private readonly Random _random = Random.Shared;
    private readonly ILogger<ThreadService> _logger;

    public event Action OnChange;

    public ThreadService(ILogger<ThreadService> logger)
    {
        _logger = logger;
        _threads = GenerateFakeThreads();
        Task.Run(ProcessThreads);
    }

    public List<BlazorThread> GetThreads() => _threads;

    private List<BlazorThread> GenerateFakeThreads()
    {
        var threads = new List<BlazorThread>();
        for (int i = 0; i < 10; i++)
        {
            int nextId = _nextId++;
            threads.Add(new BlazorThread
            {
                Id = nextId,
                Name = $"BlazorThread {nextId}",
                Status = "queued",
                Color = GetRandomColor()
            });
        }
        return threads;
    }

    private string GetRandomColor()
    {
        var colors = new[] { "red", "green", "blue", "orange", "purple" };
        return colors[_random.Next(colors.Length)];
    }

    private async Task ProcessThreads()
    {
        while (true)
        {
            if (_threads.Any(t => t.Status == "queued"))
            {
                var nextThread = _threads.First(t => t.Status == "queued");
                _logger.LogInformation($"Starting thread: {nextThread.Name}");
                nextThread.Status = "running";
                NotifyStateChanged();

                await Task.Delay(2000);

                _logger.LogInformation($"Completing thread: {nextThread.Name}");
                nextThread.Status = "completed";
                NotifyStateChanged();
            }
            else
            {
                await Task.Delay(1000);
            }
        }
    }

    private void NotifyStateChanged()
    {
        _logger.LogInformation($"State changed in {nameof(ThreadService)}");
        OnChange?.Invoke();
    }
}
