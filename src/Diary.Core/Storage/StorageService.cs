namespace Diary.Core.Storage;

/// <summary>
/// Перевіряє заповнення сховища після завантаження медіа й сповіщає користувача.
/// Сповіщення надсилається один раз при переході в стан Warning або Exceeded;
/// після повернення в Normal наступне перевищення знову дає сповіщення.
/// </summary>
public sealed class StorageService
{
    public const string WarningMessage = "Сховище заповнене на 80 % або більше";
    public const string ExceededMessage = "Сховище переповнене: нові медіафайли не завантажуються";

    private readonly INotifier _notifier;
    private readonly Dictionary<string, StorageStatus> _lastStatus = new();

    public StorageService(INotifier notifier)
    {
        _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
    }

    public StorageStatus CheckUsage(string userId, long usedBytes, bool isPremium)
    {
        var status = StorageLimit.GetStatus(usedBytes, StoragePlan.LimitFor(isPremium));
        var previous = _lastStatus.GetValueOrDefault(userId, StorageStatus.Normal);

        if (status != StorageStatus.Normal && status != previous)
            _notifier.Send(userId, status == StorageStatus.Warning ? WarningMessage : ExceededMessage);

        _lastStatus[userId] = status;
        return status;
    }
}
