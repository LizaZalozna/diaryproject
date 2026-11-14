namespace Diary.Core.Storage;

/// <summary>Ліміт сховища медіа (SCRUM-39).</summary>
public static class StorageLimit
{
    /// <summary>Від цього відсотка заповнення (включно) користувач отримує попередження.</summary>
    public const int WarningPercent = 80;

    /// <summary>Normal (&lt; 80 %), Warning (від 80 % до 100 % включно), Exceeded (&gt; 100 %).</summary>
    public static StorageStatus GetStatus(long usedBytes, long limitBytes)
    {
        EnsurePositiveLimit(limitBytes);
        if (usedBytes > limitBytes)
            return StorageStatus.Exceeded;
        if (usedBytes * 100 >= limitBytes * WarningPercent)
            return StorageStatus.Warning;
        return StorageStatus.Normal;
    }

    /// <summary>Чи можна завантажити файл, не перевищивши ліміт.</summary>
    public static bool CanUpload(long usedBytes, long fileBytes, long limitBytes)
    {
        EnsurePositiveLimit(limitBytes);
        if (fileBytes <= 0)
            throw new ValidationException("Розмір файлу має бути більшим за нуль");
        return usedBytes + fileBytes <= limitBytes;
    }

    private static void EnsurePositiveLimit(long limitBytes)
    {
        if (limitBytes <= 0)
            throw new ValidationException("Ліміт сховища має бути більшим за нуль");
    }
}
