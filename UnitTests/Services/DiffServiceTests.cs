using DiffAPI.Services;
using UnitTests.Helpers;
using Xunit;

namespace UnitTests.Services;

public class DiffServiceTests
{
    private readonly DiffService _diffService = new();

    [Fact]
    public void TestSaveLeft()
    {
        const string id = "1";
        _diffService.SaveLeft(id, "AAAAAA==");
        var diffData = DiffServiceHelper.GetInternalData(id);
        Assert.NotNull(diffData);
        Assert.Equal("AAAAAA==", diffData.Left);
    }

    [Fact]
    public void TestSaveRight()
    {
        const string id = "2";
        _diffService.SaveRight(id, "AAAAAA==");
        var diffData = DiffServiceHelper.GetInternalData(id);
        Assert.NotNull(diffData);
        Assert.Equal("AAAAAA==", diffData.Right);
    }

    [Fact]
    public void TestGetDiff_Equal()
    {
        const string id = "3";
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
        const string id = "4";
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
        const string id = "5";
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
        const string id = "6";
        _diffService.SaveLeft(id, null);
        var diffData = DiffServiceHelper.GetInternalData(id);
        Assert.Null(diffData?.Left);
    }

    [Fact]
    public void TestSaveRight_NullData()
    {
        const string id = "7";
        _diffService.SaveRight(id, null);
        var diffData = DiffServiceHelper.GetInternalData(id);
        Assert.Null(diffData?.Right);
    }
}