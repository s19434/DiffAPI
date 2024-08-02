using DiffAPI.Services;
using Xunit;

namespace UnitTests.Services;

public class DiffServiceTests
{
    private readonly DiffService _diffService = new();

    // Test to save data to the left side and verify it
    [Fact]
    public async Task TestSaveLeftAsync()
    {
        const string id = "1";
        await _diffService.SaveLeftAsync(id, "AAAAAA==");
        DiffService.DiffStore.TryGetValue(id, out var diffData);
        Assert.NotNull(diffData);
        Assert.Equal("AAAAAA==", diffData.Left);
    }

    // Test to save data to the right side and verify it
    [Fact]
    public async Task TestSaveRightAsync()
    {
        const string id = "2";
        await _diffService.SaveRightAsync(id, "AAAAAA==");
        DiffService.DiffStore.TryGetValue(id, out var diffData);
        Assert.NotNull(diffData);
        Assert.Equal("AAAAAA==", diffData.Right);
    }

    // Test to verify if two equal data parts return "Equals"
    [Fact]
    public async Task TestGetDiffAsync_Equal()
    {
        const string id = "3";
        await _diffService.SaveLeftAsync(id, "AAAAAA==");
        await _diffService.SaveRightAsync(id, "AAAAAA==");

        var (exists, diffResultType, diffs) = await _diffService.GetDiffAsync(id);
        Assert.True(exists);
        Assert.Equal("Equals", diffResultType);
        Assert.Null(diffs);
    }

    // Test to verify if different sized data parts return "SizeDoNotMatch"
    [Fact]
    public async Task TestGetDiffAsync_SizeDoNotMatch()
    {
        const string id = "4";
        await _diffService.SaveLeftAsync(id, "AAA=");
        await _diffService.SaveRightAsync(id, "AAAAAA==");

        var (exists, diffResultType, diffs) = await _diffService.GetDiffAsync(id);
        Assert.True(exists);
        Assert.Equal("SizeDoNotMatch", diffResultType);
        Assert.Null(diffs);
    }

    // Test to verify if different content data parts return "ContentDoNotMatch" with correct differences
    [Fact]
    public async Task TestGetDiffAsync_ContentDoNotMatch()
    {
        const string id = "5";
        await _diffService.SaveLeftAsync(id, "AAAAAA==");
        await _diffService.SaveRightAsync(id, "AQABAQ==");

        var (exists, diffResultType, diffs) = await _diffService.GetDiffAsync(id);
        Assert.True(exists);
        Assert.Equal("ContentDoNotMatch", diffResultType);
        Assert.NotNull(diffs);
        Assert.Equal(2, diffs.Count);

        Assert.Equal(0, diffs[0].Offset);
        Assert.Equal(1, diffs[0].Length);

        Assert.Equal(2, diffs[1].Offset);
        Assert.Equal(2, diffs[1].Length);
    }

    // Test to verify handling of null data on the left side
    [Fact]
    public async Task TestSaveLeftAsync_NullData()
    {
        const string id = "6";
        await _diffService.SaveLeftAsync(id, null);
        DiffService.DiffStore.TryGetValue(id, out var diffData);
        Assert.Null(diffData?.Left);
    }

    // Test to verify handling of null data on the right side
    [Fact]
    public async Task TestSaveRightAsync_NullData()
    {
        const string id = "7";
        await _diffService.SaveRightAsync(id, null);
        DiffService.DiffStore.TryGetValue(id, out var diffData);
        Assert.Null(diffData?.Right);
    }
}