using System.Data;
using System.Data.Common;
using System.Globalization;

namespace Diary.Core.Notes;

/// <summary>
/// Шар зберігання нотаток (сховище «Нотатки та категорії»).
/// Приймає будь-яке ADO.NET-з'єднання; у застосунку та тестах — SQLite
/// (у тестах — Microsoft.Data.Sqlite з "Data Source=:memory:").
/// </summary>
public sealed class NoteRepository
{
    private const string DateFormat = "yyyy-MM-dd'T'HH:mm:ss";
    private readonly DbConnection _connection;

    public NoteRepository(DbConnection connection)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        if (_connection.State != ConnectionState.Open)
            _connection.Open();
        Execute("""
            CREATE TABLE IF NOT EXISTS notes (
                id         INTEGER PRIMARY KEY AUTOINCREMENT,
                user_id    TEXT    NOT NULL,
                title      TEXT    NOT NULL,
                text       TEXT    NOT NULL,
                category   TEXT    NULL,
                access     INTEGER NOT NULL,
                created_at TEXT    NOT NULL
            )
            """);
    }

    /// <summary>Додає нотатку й повертає її id.</summary>
    public long Add(Note note)
    {
        using var cmd = Command("""
            INSERT INTO notes (user_id, title, text, category, access, created_at)
            VALUES (@user, @title, @text, @category, @access, @created);
            SELECT last_insert_rowid();
            """, note);
        return Convert.ToInt64(cmd.ExecuteScalar(), CultureInfo.InvariantCulture);
    }

    /// <summary>Оновлює назву, текст, категорію та доступ; false, якщо нотатки немає.</summary>
    public bool Update(Note note)
    {
        using var cmd = Command("""
            UPDATE notes SET title = @title, text = @text, category = @category, access = @access
            WHERE id = @id
            """, note);
        return cmd.ExecuteNonQuery() == 1;
    }

    /// <summary>Видаляє нотатку (SCRUM-19); якщо нотатки немає — NotFoundException.</summary>
    public void Delete(long id)
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "DELETE FROM notes WHERE id = @id";
        AddParameter(cmd, "@id", id);
        if (cmd.ExecuteNonQuery() == 0)
            throw new NotFoundException($"Нотатку з id {id} не знайдено");
    }

    public Note? GetById(long id)
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "SELECT id, user_id, title, text, category, access, created_at FROM notes WHERE id = @id";
        AddParameter(cmd, "@id", id);
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? Map(reader) : null;
    }

    /// <summary>Нотатки користувача за період [from; to), від найновіших.</summary>
    public IReadOnlyList<Note> ListByPeriod(string userId, DateTime from, DateTime to)
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = """
            SELECT id, user_id, title, text, category, access, created_at FROM notes
            WHERE user_id = @user AND created_at >= @from AND created_at < @to
            ORDER BY created_at DESC, id DESC
            """;
        AddParameter(cmd, "@user", userId);
        AddParameter(cmd, "@from", Format(from));
        AddParameter(cmd, "@to", Format(to));
        using var reader = cmd.ExecuteReader();
        var result = new List<Note>();
        while (reader.Read())
            result.Add(Map(reader));
        return result;
    }

    private DbCommand Command(string sql, Note note)
    {
        var cmd = _connection.CreateCommand();
        cmd.CommandText = sql;
        AddParameter(cmd, "@id", note.Id);
        AddParameter(cmd, "@user", note.UserId);
        AddParameter(cmd, "@title", note.Title);
        AddParameter(cmd, "@text", note.Text);
        AddParameter(cmd, "@category", (object?)note.Category ?? DBNull.Value);
        AddParameter(cmd, "@access", (int)note.Access);
        AddParameter(cmd, "@created", Format(note.CreatedAt));
        return cmd;
    }

    private void Execute(string sql)
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
    }

    private static void AddParameter(DbCommand cmd, string name, object value)
    {
        var p = cmd.CreateParameter();
        p.ParameterName = name;
        p.Value = value;
        cmd.Parameters.Add(p);
    }

    private static string Format(DateTime value) => value.ToString(DateFormat, CultureInfo.InvariantCulture);

    private static Note Map(DbDataReader r) => new(
        r.GetInt64(0),
        r.GetString(1),
        r.GetString(2),
        r.GetString(3),
        r.IsDBNull(4) ? null : r.GetString(4),
        (AccessLevel)r.GetInt64(5),
        DateTime.ParseExact(r.GetString(6), DateFormat, CultureInfo.InvariantCulture));
}
