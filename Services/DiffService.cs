using System.Collections.Concurrent;
using DiffAPI.Interfaces;
using DiffAPI.Models;

public class DiffService : IDiffService
{
    private readonly ConcurrentDictionary<string, DiffData> _diffStore = new();

    public void SaveLeft(string id, string base64Data)
    {
        var diffData = _diffStore.GetOrAdd(id, new DiffData { Id = id });
        diffData.Left = base64Data;
    }

    public void SaveRight(string id, string base64Data)
    {
        var diffData = _diffStore.GetOrAdd(id, new DiffData { Id = id });
        diffData.Right = base64Data;
    }

    public (bool Exists, string? DiffResultType, List<(int offset, int length)>? Diffs) GetDiff(string id)
    {
        if (!_diffStore.TryGetValue(id, out var diffData) || diffData.Left == null || diffData.Right == null)
        {
            return (false, null, null);
        }

        if (diffData.Left == diffData.Right)
        {
            return (true, "Equals", null);
        }

        if (diffData.Left.Length != diffData.Right.Length)
        {
            return (true, "SizeDoNotMatch", null);
        }

        var diffs = new List<(int offset, int length)>();
        for (int i = 0; i < diffData.Left.Length; i++)
        {
            if (diffData.Left[i] != diffData.Right[i])
            {
                int length = 1;
                while (i + length < diffData.Left.Length && diffData.Left[i + length] != diffData.Right[i + length])
                {
                    length++;
                }
                diffs.Add((i, length));
                i += length - 1;
            }
        }

        return (true, "ContentDoNotMatch", diffs);
    }
}