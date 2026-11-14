using System.Text.RegularExpressions;

namespace Diary.Core.Accounts;

/// <summary>Валідація даних реєстрації та підтвердження (SCRUM-14, SCRUM-17).</summary>
public sealed partial class CredentialsValidator
{
    public const int PasswordMinLength = 8;
    public const int PasswordMaxLength = 64;
    public const int CodeLength = 6;

    public void ValidatePassword(string? password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < PasswordMinLength)
            throw new ValidationException($"Пароль має містити мінімум {PasswordMinLength} символів");
        if (password.Length > PasswordMaxLength)
            throw new ValidationException($"Пароль має містити не більше {PasswordMaxLength} символів");
        if (!password.Any(char.IsLetter) || !password.Any(char.IsDigit))
            throw new ValidationException("Пароль має містити літеру і цифру");
    }

    /// <summary>Повтор пароля на формі створення нового пароля (SCRUM-17).</summary>
    public void ValidatePasswordConfirmation(string? password, string? confirmation)
    {
        if (!string.Equals(password, confirmation, StringComparison.Ordinal))
            throw new ValidationException("Паролі не збігаються");
    }

    public void ValidateEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email) || !EmailPattern().IsMatch(email.Trim()))
            throw new ValidationException("Введіть коректну електронну адресу");
    }

    /// <summary>Код підтвердження з листа — рівно 6 цифр.</summary>
    public void ValidateConfirmationCode(string? code)
    {
        if (code is null || code.Length != CodeLength || !code.All(char.IsAsciiDigit))
            throw new ValidationException($"Код має складатися з {CodeLength} цифр");
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailPattern();
}
