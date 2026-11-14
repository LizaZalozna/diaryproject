namespace Diary.Core.Notes;

public sealed record Note(
    long Id,
    string UserId,
    string Title,
    string Text,
    string? Category,
    AccessLevel Access,
    DateTime CreatedAt);
