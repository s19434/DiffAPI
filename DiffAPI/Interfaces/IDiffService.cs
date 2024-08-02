using DiffAPI.Models;

namespace DiffAPI.Interfaces;

public interface IDiffService
{
    void SaveLeft(string id, string? base64Data);
    void SaveRight(string id, string? base64Data);
    (bool Exists, string? DiffResultType, List<Diff>? Diffs) GetDiff(string id);
}