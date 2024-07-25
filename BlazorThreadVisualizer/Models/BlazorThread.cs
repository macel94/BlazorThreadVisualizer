namespace BlazorThreadVisualizer.Models;

public class BlazorThread
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Status { get; set; } // "queued", "running", "completed"
    public string? Error { get; set; }
}
