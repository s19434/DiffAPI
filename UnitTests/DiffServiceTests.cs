using DiffAPI.Interfaces;
using DiffAPI.Services;
using Xunit;

namespace DiffAPI.UnitTests;

public class DiffServiceTests
{
    private readonly IDiffService _diffService = new DiffService();

    [Fact]
    public void TestSaveLeft()
    {
        _diffService.SaveLeft("1", "AAAAAA==");
        var (exists, _, _) = _diffService.GetDiff("1");
        Assert.True(exists);
    }

    [Fact]
    public void TestSaveRight()
    {
        _diffService.SaveRight("1", "AAAAAA==");
        var (exists, _, _) = _diffService.GetDiff("1");
        Assert.True(exists);
    }

    [Fact]
    public void TestGetDiff_Equal()
    {
        _diffService.SaveLeft("1", "AAAAAA==");
        _diffService.SaveRight("1", "AAAAAA==");

        var (exists, diffResultType, diffs) = _diffService.GetDiff("1");
        Assert.True(exists);
        Assert.Equal("Equals", diffResultType);
        Assert.Null(diffs);
    }

    [Fact]
    public void TestGetDiff_SizeDoNotMatch()
    {
        _diffService.SaveLeft("1", "AAA=");
        _diffService.SaveRight("1", "AAAAAA==");

        var (exists, diffResultType, diffs) = _diffService.GetDiff("1");
        Assert.True(exists);
        Assert.Equal("SizeDoNotMatch", diffResultType);
        Assert.Null(diffs);
    }

    [Fact]
    public void TestGetDiff_ContentDoNotMatch()
    {
        _diffService.SaveLeft("1", "AAAAAA==");
        _diffService.SaveRight("1", "AQABAQ==");

        var (exists, diffResultType, diffs) = _diffService.GetDiff("1");
        Assert.True(exists);
        Assert.Equal("ContentDoNotMatch", diffResultType);
        Assert.NotNull(diffs);
        Assert.Equal(2, diffs.Count);

        Assert.Equal(0, diffs[0].Offset);
        Assert.Equal(1, diffs[0].Length);

        Assert.Equal(2, diffs[1].Offset);
        Assert.Equal(2, diffs[1].Length);
    }
}