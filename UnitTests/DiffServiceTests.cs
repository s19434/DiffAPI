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
        string id = "1";
        _diffService.SaveLeft(id, "AAAAAA==");
        var (exists, _, _) = _diffService.GetDiff(id);
        Assert.True(exists);
    }

    [Fact]
    public void TestSaveRight()
    {
        string id = "2";
        _diffService.SaveRight(id, "AAAAAA==");
        var (exists, _, _) = _diffService.GetDiff(id);
        Assert.True(exists);
    }

    [Fact]
    public void TestGetDiff_Equal()
    {
        string id = "3";
        _diffService.SaveLeft(id, "AAAAAA==");
        _diffService.SaveRight(id, "AAAAAA==");

        var (exists, diffResultType, diffs) = _diffService.GetDiff(id);
        Assert.True(exists);
        Assert.Equal("Equals", diffResultType);
        Assert.Null(diffs);
    }

    [Fact]
    public void TestGetDiff_SizeDoNotMatch()
    {
        string id = "4";
        _diffService.SaveLeft(id, "AAA=");
        _diffService.SaveRight(id, "AAAAAA==");

        var (exists, diffResultType, diffs) = _diffService.GetDiff(id);
        Assert.True(exists);
        Assert.Equal("SizeDoNotMatch", diffResultType);
        Assert.Null(diffs);
    }

    [Fact]
    public void TestGetDiff_ContentDoNotMatch()
    {
        string id = "5";
        _diffService.SaveLeft(id, "AAAAAA==");
        _diffService.SaveRight(id, "AQABAQ==");

        var (exists, diffResultType, diffs) = _diffService.GetDiff(id);
        Assert.True(exists);
        Assert.Equal("ContentDoNotMatch", diffResultType);
        Assert.NotNull(diffs);
        Assert.Equal(2, diffs.Count);

        Assert.Equal(0, diffs[0].Offset);
        Assert.Equal(1, diffs[0].Length);

        Assert.Equal(2, diffs[1].Offset);
        Assert.Equal(2, diffs[1].Length);
    }

    [Fact]
    public void TestSaveLeft_NullData()
    {
        string id = "6";
        Assert.Throws<ArgumentNullException>(() => _diffService.SaveLeft(id, null));
    }

    [Fact]
    public void TestSaveRight_NullData()
    {
        string id = "7";
        Assert.Throws<ArgumentNullException>(() => _diffService.SaveRight(id, null));
    }
}