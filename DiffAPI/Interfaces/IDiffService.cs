using DiffAPI.Models;

namespace DiffAPI.Interfaces;

public interface IDiffService
{
    Task SaveLeftAsync(string id, string? base64Data);
    Task SaveRightAsync(string id, string? base64Data);
    Task<(bool Exists, string? DiffResultType, List<Diff>? Diffs)> GetDiffAsync(string id);
}