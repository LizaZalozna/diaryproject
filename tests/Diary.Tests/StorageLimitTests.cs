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
}
