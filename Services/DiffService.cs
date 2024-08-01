using System.Collections.Concurrent;
using DiffAPI.Interfaces;
using DiffAPI.Models;

namespace DiffAPI.Services;

public class DiffService : IDiffService
{
    private readonly ConcurrentDictionary<string, DiffData> _diffStore = new ConcurrentDictionary<string, DiffData>();

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

    public (bool Exists, string? DiffResultType, List<Diff>? Diffs) GetDiff(string id)
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

        var diffs = new List<Diff>();
        int? currentOffset = null;
        int currentLength = 0;

        for (int i = 0; i < diffData.Left.Length; i++)
        {
            if (diffData.Left[i] != diffData.Right[i])
            {
                if (currentOffset == null)
                {
                    currentOffset = i;
                }
                currentLength++;
            }
            else
            {
                if (currentOffset != null)
                {
                    diffs.Add(new Diff { Offset = currentOffset.Value, Length = currentLength });
                    currentOffset = null;
                    currentLength = 0;
                }
            }
        }

        // Add the last diff if it exists
        if (currentOffset != null)
        {
            diffs.Add(new Diff { Offset = currentOffset.Value, Length = currentLength });
        }

        return (true, "ContentDoNotMatch", diffs);
    }
}
