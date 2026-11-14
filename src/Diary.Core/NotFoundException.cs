namespace Diary.Core;

/// <summary>Запис з указаним id не знайдено.</summary>
public sealed class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}
