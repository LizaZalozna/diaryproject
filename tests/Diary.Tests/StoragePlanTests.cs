using Diary.Core.Storage;

namespace Diary.Tests;

public class StoragePlanTests
{
    [Theory]
    [InlineData(false, StoragePlan.FreeLimitBytes)]
    [InlineData(true, StoragePlan.PremiumLimitBytes)]
    public void LimitFor_Subscription_ReturnsPlanLimit(bool isPremium, long expected)
    {
        var limit = StoragePlan.LimitFor(isPremium);

        Assert.Equal(expected, limit);
    }

    [Fact]
    public void GetStatus_FreeUserAt80Percent_IsWarning()
    {
        var used = StoragePlan.FreeLimitBytes * 80 / 100;

        var status = StorageLimit.GetStatus(used, StoragePlan.LimitFor(isPremium: false));

        Assert.Equal(StorageStatus.Warning, status);
    }
}
