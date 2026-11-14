using Diary.Core;
using Diary.Core.Storage;

namespace Diary.Tests;

public class StorageLimitTests
{
    [Theory]
    [InlineData(0, 1000, StorageStatus.Normal)]
    [InlineData(799, 1000, StorageStatus.Normal)]
    [InlineData(800, 1000, StorageStatus.Warning)]
    [InlineData(1000, 1000, StorageStatus.Warning)]
    [InlineData(1001, 1000, StorageStatus.Exceeded)]
    public void GetStatus_UsageAroundBoundaries_ReturnsExpectedStatus(long used, long limit, StorageStatus expected)
    {
        var status = StorageLimit.GetStatus(used, limit);

        Assert.Equal(expected, status);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void GetStatus_NonPositiveLimit_ThrowsValidationException(long limit)
    {
        Assert.Throws<ValidationException>(() => StorageLimit.GetStatus(10, limit));
    }

    [Theory]
    [InlineData(900, 99, 1000, true)]
    [InlineData(900, 100, 1000, true)]
    [InlineData(900, 101, 1000, false)]
    [InlineData(1000, 1, 1000, false)]
    public void CanUpload_FileAroundRemainingSpace_ReturnsExpected(long used, long file, long limit, bool expected)
    {
        var allowed = StorageLimit.CanUpload(used, file, limit);

        Assert.Equal(expected, allowed);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void CanUpload_NonPositiveFileSize_ThrowsValidationException(long file)
    {
        Assert.Throws<ValidationException>(() => StorageLimit.CanUpload(0, file, 1000));
    }
}
