namespace Intap.FirstProject.Domain.Tasks;

/// <summary>
/// Parameter object that groups the mutable data fields of a <see cref="TaskEntry"/>.
/// Introduced to reduce the constructor/factory parameter count (S107).
/// </summary>
public sealed record TaskEntryData(
    string     Description,
    DateOnly   Date,
    TimeOnly   StartTime,
    TimeOnly   EndTime,
    string     Duration,
    TaskStatus Status,
    string?    Observations);
