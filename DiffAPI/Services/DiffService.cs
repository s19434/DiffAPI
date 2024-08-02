using System.Collections.Concurrent;
using DiffAPI.Interfaces;
using DiffAPI.Models;

namespace DiffAPI.Services;

public class DiffService : IDiffService
{
    public static readonly ConcurrentDictionary<string, DiffData> DiffStore = new();

    public Task SaveLeftAsync(string id, string? base64Data)
    {
        var diffData = DiffStore.GetOrAdd(id, new DiffData { Id = id });
        diffData.Left = base64Data;
        return Task.CompletedTask;
    }

    public Task SaveRightAsync(string id, string? base64Data)
    {
        var diffData = DiffStore.GetOrAdd(id, new DiffData { Id = id });
        diffData.Right = base64Data;
        return Task.CompletedTask;
    }

    // Asynchronously get the differences between left and right data
    public Task<(bool Exists, string? DiffResultType, List<Diff>? Diffs)> GetDiffAsync(string id)
    {
        if (!DiffStore.TryGetValue(id, out var diffData) || diffData.Left == null || diffData.Right == null)
        {
            return Task.FromResult<(bool, string?, List<Diff>?)>((false, null, null));
        }

        if (diffData.Left == diffData.Right)
        {
            return Task.FromResult<(bool, string?, List<Diff>?)>((true, "Equals", null));
        }

        var leftBytes = Convert.FromBase64String(diffData.Left);
        var rightBytes = Convert.FromBase64String(diffData.Right);

        if (leftBytes.Length != rightBytes.Length)
        {
            return Task.FromResult<(bool, string?, List<Diff>?)>((true, "SizeDoNotMatch", null));
        }

        var diffs = new List<Diff>();
        int? currentOffset = null;
        int currentLength = 0;

        // Calculate differences
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

        return Task.FromResult<(bool, string?, List<Diff>?)>((true, "ContentDoNotMatch", diffs));
    }
}