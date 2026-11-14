namespace Diary.Core.Storage;

/// <summary>Розмір сховища медіа залежно від підписки.</summary>
public static class StoragePlan
{
    public const long FreeLimitBytes = 100L * 1024 * 1024;      // 100 МБ
    public const long PremiumLimitBytes = 1024L * 1024 * 1024;  // 1 ГБ

    public static long LimitFor(bool isPremium) => isPremium ? PremiumLimitBytes : FreeLimitBytes;
}
