namespace Diary.Core.Notes;

/// <summary>Валідація полів нотатки (SCRUM-7, SCRUM-24).</summary>
public sealed class NoteValidator
{
    public const int TitleMaxLength = 100;
    public const int TextMaxLength = 5000;

    /// <summary>Категорії, які пропонує форма створення нотатки.</summary>
    public static readonly IReadOnlyList<string> Categories = ["Personal", "Study", "Work", "Ideas"];

    /// <summary>Повертає назву без пробілів на краях.</summary>
    public string ValidateTitle(string? title)
    {
        var trimmed = title?.Trim() ?? string.Empty;
        if (trimmed.Length == 0)
            throw new ValidationException("Введіть назву нотатки");
        if (trimmed.Length > TitleMaxLength)
            throw new ValidationException($"Назва має містити не більше {TitleMaxLength} символів");
        return trimmed;
    }

    /// <summary>Текст обов'язковий.</summary>
    public string ValidateText(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ValidationException("Введіть текст нотатки");
        var value = text;
        if (value.Length > TextMaxLength)
            throw new ValidationException($"Текст має містити не більше {TextMaxLength} символів");
        return value;
    }

    /// <summary>Категорія обов'язкова й має бути зі списку; повертає назву у написанні зі списку.</summary>
    public string ValidateCategory(string? category)
    {
        var match = Categories.FirstOrDefault(c => string.Equals(c, category?.Trim(), StringComparison.OrdinalIgnoreCase));
        return match ?? throw new ValidationException("Оберіть категорію");
    }

    /// <summary>Групові нотатки доступні лише з Premium.</summary>
    public void ValidateAccess(AccessLevel access, bool isPremium)
    {
        if (!Enum.IsDefined(access))
            throw new ValidationException("Оберіть тип доступу");
        if (access == AccessLevel.Group && !isPremium)
            throw new ValidationException("Групові нотатки доступні лише з Premium");
    }
}
