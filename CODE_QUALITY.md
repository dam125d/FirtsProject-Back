# CODE_QUALITY.md — Backend Quality Rules

Rules derived from SonarQube issues found and fixed in this project.

---

## CancellationToken (S927 + S1006)

Always match the interface parameter name **and** default value exactly.

```csharp
// Interface
Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

// Implementation — must match name AND default
public Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => ...
```

Never use `ct`, `token`, or any abbreviation. Never omit `= default` if the interface has it.

---

## Too Many Parameters (S107)

Max 7 parameters per method. Group related fields into a Parameter Object.

```csharp
// Bad — 9 parameters
public static TaskEntry Create(Guid projectId, Guid userId, string desc, DateOnly date, ...)

// Good — Parameter Object
public sealed record TaskEntryData(string Description, DateOnly Date, TimeOnly Start, ...);
public static TaskEntry Create(Guid projectId, Guid userId, TaskEntryData data)
```

---

## CQRS Phantom Type Parameters (S2326)

`IQuery<TResponse>` and `ICommand<TResponse>` intentionally don't use `TResponse` in their body — it flows to the handler. Suppress with justification:

```csharp
[SuppressMessage("Design", "S2326",
    Justification = "Phantom type: TResponse flows to IQueryHandler<TQuery, TResponse>.")]
public interface IQuery<TResponse> { }
```

---

## Method Overload Grouping (S4136)

Keep all overloads of the same method adjacent:

```csharp
// Correct
public static Result Success() => ...
public static Result<T> Success<T>(T value) => ...
public static Result Failure(ErrorResult error) => ...
public static Result<T> Failure<T>(ErrorResult error) => ...
```

---

## Utility / Partial Classes (S1118 + ASP0027)

Classes not meant to be instantiated need `protected` constructor or `static`.  
For the `Program` partial class required by `WebApplicationFactory`:

```csharp
public partial class Program
{
    protected Program() { }   // satisfies S1118, keeps E2E compatibility
}
```

---

## Placeholder Tests (S2699)

Delete `UnitTest1.cs` and any test method without at least one assertion. Every `[Fact]` must assert something.

---

## Static Arrays (CA1861)

In EF Core migrations and tests, avoid repeated inline arrays. Use `static readonly`:

```csharp
// Bad
migrationBuilder.InsertData(columns: new[] { "Id", "Name" }, ...);

// Good
private static readonly string[] Columns = ["Id", "Name"];
migrationBuilder.InsertData(columns: Columns, ...);
```

---

## String Comparisons (CA1862)

Never use `.ToLower()` / `.ToUpper()` for comparisons. Use `StringComparison`:

```csharp
// Bad
u.Email == email.ToLowerInvariant()

// Good
string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase)
```

---

## Logging (CA1873)

Use `IsEnabled` guard before building expensive log messages:

```csharp
// Bad
_logger.LogInformation($"Seeding {items.Count} records...");

// Good
if (_logger.IsEnabled(LogLevel.Information))
    _logger.LogInformation("Seeding {Count} records...", items.Count);
```
