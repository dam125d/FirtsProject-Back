using Intap.FirstProject.Domain.Common;
using Intap.FirstProject.Domain.Projects;
using Intap.FirstProject.Domain.Users;

namespace Intap.FirstProject.Domain.Tasks;

public sealed class TaskEntry : BaseEntity
{
    private TaskEntry() { }

    public Guid       ProjectId    { get; private set; }
    public Guid       UserId       { get; private set; }
    public string     Description  { get; private set; } = string.Empty;
    public DateOnly   Date         { get; private set; }
    public TimeOnly   StartTime    { get; private set; }
    public TimeOnly   EndTime      { get; private set; }
    public string     Duration     { get; private set; } = string.Empty;
    public TaskStatus Status       { get; private set; } = TaskStatus.Pending;
    public string?    Observations { get; private set; }
    public bool       IsDeleted    { get; private set; }

    // Navigation
    public Project Project { get; private set; } = null!;
    public User    User    { get; private set; } = null!;

    public static TaskEntry Create(Guid projectId, Guid userId, TaskEntryData data)
    {
        return new TaskEntry
        {
            ProjectId    = projectId,
            UserId       = userId,
            Description  = data.Description,
            Date         = data.Date,
            StartTime    = data.StartTime,
            EndTime      = data.EndTime,
            Duration     = data.Duration,
            Status       = data.Status,
            Observations = data.Observations,
        };
    }

    public void Update(TaskEntryData data)
    {
        Description  = data.Description;
        Date         = data.Date;
        StartTime    = data.StartTime;
        EndTime      = data.EndTime;
        Duration     = data.Duration;
        Status       = data.Status;
        Observations = data.Observations;
        SetUpdatedAt();
    }

    public void Delete()
    {
        IsDeleted = true;
        SetUpdatedAt();
    }
}
