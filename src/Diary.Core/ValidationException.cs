namespace Diary.Core;

/// <summary>Помилка валідації вхідних даних; Message — текст для користувача (як у станах Error прототипу Figma).</summary>
public sealed class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
}
