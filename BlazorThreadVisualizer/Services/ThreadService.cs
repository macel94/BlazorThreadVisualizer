using BlazorThreadVisualizer.Models;

public class ThreadService
{
    private List<BlazorThread> _threads;
    private int _nextId = 0;
    private readonly Random _random = Random.Shared;
    private readonly ILogger<ThreadService> _logger;
    private Task? _processingTask;
    private CancellationTokenSource _cancellationTokenSource;

    public event Action OnChange;

    public ThreadService(ILogger<ThreadService> logger)
    {
        _logger = logger;
        _threads = GenerateFakeThreads();
        _cancellationTokenSource = new CancellationTokenSource();
        _processingTask = Task.Run(() => ProcessThreads(_cancellationTokenSource.Token));
    }

    public List<BlazorThread> GetThreads() => _threads;

    private List<BlazorThread> GenerateFakeThreads()
    {
        var threads = new List<BlazorThread>();
        for (int i = 0; i < 5; i++)
        {
            int nextId = _nextId++;
            threads.Add(new BlazorThread
            {
                Id = nextId,
                Name = $"BlazorThread {nextId}",
                Status = "queued",
            });
        }
        return threads;
    }

    private async Task ProcessThreads(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (_threads.Any(t => t.Status == "queued"))
            {
                var nextThread = _threads.First(t => t.Status == "queued");
                _logger.LogInformation($"Starting thread: {nextThread.Name}");
                nextThread.Status = "running";
                NotifyStateChanged();

                await Task.Delay(2000, cancellationToken);

                _logger.LogInformation($"Completing thread: {nextThread.Name}");
                nextThread.Status = "completed";
                NotifyStateChanged();
            }
            else
            {
                await Task.Delay(1000, cancellationToken);
            }
        }
    }

    public void RestartQueue()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        _threads = GenerateFakeThreads();
        _processingTask = Task.Run(() => ProcessThreads(_cancellationTokenSource.Token));
        NotifyStateChanged();
    }

    private void NotifyStateChanged()
    {
        _logger.LogInformation($"State changed in {nameof(ThreadService)}");
        OnChange?.Invoke();
    }
}
