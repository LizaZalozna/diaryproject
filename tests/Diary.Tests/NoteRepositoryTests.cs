using Diary.Core;
using Diary.Core.Notes;
using Microsoft.Data.Sqlite;

namespace Diary.Tests;

/// <summary>Інтеграційні тести NoteRepository на SQLite у пам'яті.</summary>
public sealed class NoteRepositoryTests : IDisposable
{
    private const string UserId = "user-1";

    private readonly SqliteConnection _connection;
    private readonly NoteRepository _repository;

    // Фікстура: нова порожня БД у пам'яті для кожного тесту.
    public NoteRepositoryTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();
        _repository = new NoteRepository(_connection);
    }

    // Очищення: закриваємо з'єднання після кожного тесту, БД у пам'яті зникає разом з ним.
    public void Dispose() => _connection.Dispose();

    private static Note NewNote(string title, DateTime createdAt, string userId = UserId) =>
        new(0, userId, title, "Текст нотатки", "Study", AccessLevel.Private, createdAt);

    [Fact] // TC-23
    public void Add_ValidNote_ReturnsIdAndNoteIsStored()
    {
        var note = NewNote("Мій день", new DateTime(2026, 11, 10, 9, 30, 0));

        var id = _repository.Add(note);

        var stored = _repository.GetById(id);
        Assert.NotNull(stored);
        Assert.Equal(note with { Id = id }, stored);
    }

    [Fact] // TC-24
    public void Update_ExistingNote_ChangesTitle()
    {
        var id = _repository.Add(NewNote("Стара", new DateTime(2026, 11, 10)));
        var changed = _repository.GetById(id)! with { Title = "Нова" };

        var updated = _repository.Update(changed);

        Assert.True(updated);
        Assert.Equal("Нова", _repository.GetById(id)!.Title);
    }

    [Fact]
    public void Update_MissingNote_ReturnsFalse()
    {
        var missing = NewNote("Немає", new DateTime(2026, 11, 10)) with { Id = 999 };

        var updated = _repository.Update(missing);

        Assert.False(updated);
    }

    [Fact] // TC-25
    public void Delete_ExistingNote_RemovesItFromDatabase()
    {
        var id = _repository.Add(NewNote("Видалити", new DateTime(2026, 11, 10)));

        _repository.Delete(id);

        Assert.Null(_repository.GetById(id));
    }

    [Fact] // TC-26
    public void Delete_MissingNote_ThrowsNotFoundException()
    {
        Assert.Throws<NotFoundException>(() => _repository.Delete(999));
    }

    [Fact] // TC-27
    public void ListByPeriod_EmptyDatabase_ReturnsEmptyList()
    {
        var notes = _repository.ListByPeriod(UserId, new DateTime(2026, 11, 1), new DateTime(2026, 12, 1));

        Assert.Empty(notes);
    }

    [Fact] // TC-28
    public void ListByPeriod_NotesOnDifferentDates_ReturnsOnlyPeriodNewestFirst()
    {
        _repository.Add(NewNote("Жовтень", new DateTime(2026, 10, 31, 23, 59, 59)));
        _repository.Add(NewNote("Перше листопада", new DateTime(2026, 11, 1)));
        _repository.Add(NewNote("Середина", new DateTime(2026, 11, 15)));
        _repository.Add(NewNote("Чужа", new DateTime(2026, 11, 20), userId: "user-2"));
        _repository.Add(NewNote("Грудень", new DateTime(2026, 12, 1)));

        var titles = _repository
            .ListByPeriod(UserId, new DateTime(2026, 11, 1), new DateTime(2026, 12, 1))
            .Select(n => n.Title);

        Assert.Equal(["Середина", "Перше листопада"], titles);
    }

    [Fact] // TC-29: наскрізний тест «бізнес-логіка → репозиторій → БД»
    public void ValidatedNote_AddedThenDeleted_ListIsEmpty()
    {
        var validator = new NoteValidator();
        var note = new Note(
            0,
            UserId,
            validator.ValidateTitle("  Конспект  "),
            validator.ValidateText("Текст лекції"),
            validator.ValidateCategory("study"),
            AccessLevel.Private,
            new DateTime(2026, 11, 10));
        var period = (From: new DateTime(2026, 11, 1), To: new DateTime(2026, 12, 1));

        var id = _repository.Add(note);
        var afterAdd = _repository.ListByPeriod(UserId, period.From, period.To);
        _repository.Delete(id);
        var afterDelete = _repository.ListByPeriod(UserId, period.From, period.To);

        var saved = Assert.Single(afterAdd);
        Assert.Equal("Конспект", saved.Title);
        Assert.Equal("Study", saved.Category);
        Assert.Empty(afterDelete);
    }
}
