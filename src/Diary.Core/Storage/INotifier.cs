namespace Diary.Core.Storage;

/// <summary>Надсилання сповіщень користувачу (у застосунку — через пакет «Інтеграції» / поштовий сервіс).</summary>
public interface INotifier
{
    void Send(string userId, string message);
}
