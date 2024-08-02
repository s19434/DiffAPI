using System.Collections.Concurrent;
using DiffAPI.Models;

namespace DiffAPI.UnitTests;

public static class DiffServiceHelper
{
    public static DiffData GetInternalData(string id)
    {
        var field = typeof(Services.DiffService).GetField("DiffStore",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        var diffStore = (ConcurrentDictionary<string, DiffData>)field.GetValue(null);
        diffStore.TryGetValue(id, out var diffData);
        return diffData;
    }
}