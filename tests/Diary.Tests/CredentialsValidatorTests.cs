using Diary.Core;
using Diary.Core.Accounts;

namespace Diary.Tests;

public class CredentialsValidatorTests
{
    private readonly CredentialsValidator _validator = new();

    [Theory]
    [InlineData("Qwerty12")]                                                          // 8 символів — нижня межа
    [InlineData("пароль2026")]                                                        // кирилиця
    [InlineData("a1234567890123456789012345678901234567890123456789012345678901bc")] // 64 символи — верхня межа
    public void ValidatePassword_ValidPassword_DoesNotThrow(string password)
    {
        var ex = Record.Exception(() => _validator.ValidatePassword(password));

        Assert.Null(ex);
    }

    [Theory]
    [InlineData(null, "Пароль має містити мінімум 8 символів")]
    [InlineData("", "Пароль має містити мінімум 8 символів")]
    [InlineData("Qwerty1", "Пароль має містити мінімум 8 символів")]                                                              // 7 символів
    [InlineData("a12345678901234567890123456789012345678901234567890123456789012bc", "Пароль має містити не більше 64 символів")] // 65 символів
    [InlineData("abcdefgh", "Пароль має містити літеру і цифру")]
    [InlineData("12345678", "Пароль має містити літеру і цифру")]
    public void ValidatePassword_InvalidPassword_ThrowsWithMessage(string? password, string expectedMessage)
    {
        var ex = Assert.Throws<ValidationException>(() => _validator.ValidatePassword(password));

        Assert.Equal(expectedMessage, ex.Message);
    }

    [Fact]
    public void ValidatePasswordConfirmation_SameValues_DoesNotThrow()
    {
        var ex = Record.Exception(() => _validator.ValidatePasswordConfirmation("Qwerty123", "Qwerty123"));

        Assert.Null(ex);
    }

    [Theory]
    [InlineData("Qwerty123", "Qwerty124")]
    [InlineData("Qwerty123", "qwerty123")] // регістр враховується
    [InlineData("Qwerty123", null)]
    public void ValidatePasswordConfirmation_DifferentValues_ThrowsWithMessage(string? password, string? confirmation)
    {
        var ex = Assert.Throws<ValidationException>(() => _validator.ValidatePasswordConfirmation(password, confirmation));

        Assert.Equal("Паролі не збігаються", ex.Message);
    }

    [Theory]
    [InlineData("user@example.com")]
    [InlineData("  student@univ.edu.ua  ")]
    public void ValidateEmail_ValidEmail_DoesNotThrow(string email)
    {
        var ex = Record.Exception(() => _validator.ValidateEmail(email));

        Assert.Null(ex);
    }

    [Theory]
    [InlineData("")]
    [InlineData("user@")]
    [InlineData("user.example.com")]
    [InlineData("us er@example.com")]
    public void ValidateEmail_InvalidEmail_Throws(string email)
    {
        Assert.Throws<ValidationException>(() => _validator.ValidateEmail(email));
    }

    [Fact]
    public void ValidateConfirmationCode_SixDigits_DoesNotThrow()
    {
        var ex = Record.Exception(() => _validator.ValidateConfirmationCode("048213"));

        Assert.Null(ex);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("12345")]
    [InlineData("1234567")]
    [InlineData("12a456")]
    [InlineData("１２３４５６")] // повноширинні цифри — не ASCII
    public void ValidateConfirmationCode_InvalidCode_ThrowsWithMessage(string? code)
    {
        var ex = Assert.Throws<ValidationException>(() => _validator.ValidateConfirmationCode(code));

        Assert.Equal("Код має складатися з 6 цифр", ex.Message);
    }
}
