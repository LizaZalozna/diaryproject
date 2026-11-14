using Diary.Core;
using Diary.Core.Notes;

namespace Diary.Tests;

public class NoteValidatorTests
{
    // Фікстура: xUnit створює новий екземпляр класу (і валідатора) для кожного тесту.
    private readonly NoteValidator _validator = new();

    [Fact]
    public void ValidateTitle_TitleWithSpaces_ReturnsTrimmedTitle()
    {
        // Arrange
        const string title = "  Конспект лекції  ";

        // Act
        var result = _validator.ValidateTitle(title);

        // Assert
        Assert.Equal("Конспект лекції", result);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(NoteValidator.TitleMaxLength)]
    public void ValidateTitle_LengthWithinBoundaries_IsAccepted(int length)
    {
        var title = new string('а', length);

        var result = _validator.ValidateTitle(title);

        Assert.Equal(length, result.Length);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ValidateTitle_EmptyTitle_ThrowsWithFigmaMessage(string? title)
    {
        var ex = Assert.Throws<ValidationException>(() => _validator.ValidateTitle(title));

        Assert.Equal("Введіть назву нотатки", ex.Message);
    }

    [Fact]
    public void ValidateTitle_TitleLongerThanMax_ThrowsWithMessage()
    {
        var title = new string('а', NoteValidator.TitleMaxLength + 1);

        var ex = Assert.Throws<ValidationException>(() => _validator.ValidateTitle(title));

        Assert.Equal("Назва має містити не більше 100 символів", ex.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  \n ")]
    public void ValidateText_EmptyText_ThrowsWithMessage(string? text)
    {
        var ex = Assert.Throws<ValidationException>(() => _validator.ValidateText(text));

        Assert.Equal("Введіть текст нотатки", ex.Message);
    }

    [Fact]
    public void ValidateText_TextOfMaxLength_IsAccepted()
    {
        var text = new string('т', NoteValidator.TextMaxLength);

        var result = _validator.ValidateText(text);

        Assert.Equal(NoteValidator.TextMaxLength, result.Length);
    }

    [Fact]
    public void ValidateText_TextLongerThanMax_Throws()
    {
        var text = new string('т', NoteValidator.TextMaxLength + 1);

        Assert.Throws<ValidationException>(() => _validator.ValidateText(text));
    }

    [Theory]
    [InlineData(AccessLevel.Private, false)]
    [InlineData(AccessLevel.Public, false)]
    [InlineData(AccessLevel.Group, true)]
    public void ValidateAccess_AllowedCombination_DoesNotThrow(AccessLevel access, bool isPremium)
    {
        var ex = Record.Exception(() => _validator.ValidateAccess(access, isPremium));

        Assert.Null(ex);
    }

    [Fact]
    public void ValidateAccess_GroupWithoutPremium_Throws()
    {
        var ex = Assert.Throws<ValidationException>(() => _validator.ValidateAccess(AccessLevel.Group, isPremium: false));

        Assert.Contains("Premium", ex.Message);
    }

    [Fact]
    public void ValidateAccess_UndefinedValue_ThrowsWithMessage()
    {
        var ex = Assert.Throws<ValidationException>(() => _validator.ValidateAccess((AccessLevel)42, isPremium: true));

        Assert.Equal("Оберіть тип доступу", ex.Message);
    }
}
