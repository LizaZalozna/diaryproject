# Цифровий щоденник — Diary.Core

Модуль бізнес-логіки «Цифрового щоденника» і автоматизовані тести до нього.
Команда КС_23_team6, лабораторна робота «Автоматизоване тестування ПЗ».

## Що реалізовано

| Компонент | Призначення | Історія Jira |
|---|---|---|
| `NoteValidator` | назва і текст нотатки, рівень доступу | SCRUM-7, SCRUM-24 |
| `CredentialsValidator` | пароль і його повтор, e-mail, код підтвердження | SCRUM-14, SCRUM-17 |
| `NoteRepository` | збереження, редагування, видалення, список нотаток (SQLite) | SCRUM-7, SCRUM-11, SCRUM-13, SCRUM-19 |
| `StorageLimit`, `StorageService` | ліміт сховища медіа і сповіщення через `INotifier` | SCRUM-39 |

## Запуск

Потрібен .NET SDK 8.0. Команди однакові для Windows, Linux і macOS:

```bash
dotnet test
```

Запуск одного класу тестів:

```bash
dotnet test --filter "FullyQualifiedName~NoteValidatorTests"
```

## Структура

```
src/Diary.Core/     код модуля
tests/Diary.Tests/  тести (xUnit)
docs/               середовище, виведення тестів для етапів TDD
```
