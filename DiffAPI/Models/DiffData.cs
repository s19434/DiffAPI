namespace DiffAPI.Models;

public class DiffData
{
    public string Id { get; set; } = null!;
    public string? Left { get; set; }
    public string? Right { get; set; }
}