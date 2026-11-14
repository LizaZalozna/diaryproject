namespace Diary.Core.Storage;

/// <summary>Ліміт сховища медіа (SCRUM-39).</summary>
public static class StorageLimit
{
    public static StorageStatus GetStatus(long usedBytes, long limitBytes)
    {
        if (usedBytes > limitBytes)
            return StorageStatus.Exceeded;
        if (usedBytes * 100 >= limitBytes * 80)
            return StorageStatus.Warning;
        return StorageStatus.Normal;
    }
}
