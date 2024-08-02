using System.Collections.Concurrent;
using DiffAPI.Interfaces;
using DiffAPI.Models;

namespace DiffAPI.Services;

public class DiffService : IDiffService
{
    private static readonly ConcurrentDictionary<string, DiffData> DiffStore = new();

    public void SaveLeft(string id, string base64Data)
    {
        var diffData = DiffStore.GetOrAdd(id, new DiffData { Id = id });
        diffData.Left = base64Data;
    }

    public void SaveRight(string id, string base64Data)
    {
        var diffData = DiffStore.GetOrAdd(id, new DiffData { Id = id });
        diffData.Right = base64Data;
    }

    public (bool Exists, string? DiffResultType, List<Diff>? Diffs) GetDiff(string id)
    {
        if (!DiffStore.TryGetValue(id, out var diffData) || diffData.Left == null || diffData.Right == null)
        {
            return (false, null, null);
        }

        if (diffData.Left == diffData.Right)
        {
            return (true, "Equals", null);
        }

        var leftBytes = Convert.FromBase64String(diffData.Left);
        var rightBytes = Convert.FromBase64String(diffData.Right);

        if (leftBytes.Length != rightBytes.Length)
        {
            return (true, "SizeDoNotMatch", null);
        }

        var diffs = new List<Diff>();
        int? currentOffset = null;
        int currentLength = 0;

        for (int i = 0; i < leftBytes.Length; i++)
        {
            if (leftBytes[i] != rightBytes[i])
            {
                if (currentOffset == null)
                {
                    currentOffset = i;
                    currentLength = 1;
                }
                else
                {
                    currentLength++;
                }
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