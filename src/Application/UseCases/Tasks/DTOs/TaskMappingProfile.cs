using AutoMapper;
using Intap.FirstProject.Domain.Tasks;

namespace Intap.FirstProject.Application.UseCases.Tasks.DTOs;

public sealed class TaskMappingProfile : Profile
{
    public TaskMappingProfile()
    {
        CreateMap<TaskEntry, TaskDto>()
            .ConvertUsing((s, _, _) => new TaskDto(
                s.Id,
                s.ProjectId,
                s.Project.Name,
                s.UserId,
                s.User.FullName,
                s.Description,
                s.Date.ToString("yyyy-MM-dd"),
                s.StartTime.ToString("HH:mm"),
                s.EndTime.ToString("HH:mm"),
                s.Duration,
                MapTaskStatus(s.Status),
                s.Observations
            ));
    }

    private static string MapTaskStatus(Domain.Tasks.TaskStatus status) => status switch
    {
        Domain.Tasks.TaskStatus.Pending    => "pending",
        Domain.Tasks.TaskStatus.InProgress => "in-progress",
        Domain.Tasks.TaskStatus.Completed  => "completed",
        Domain.Tasks.TaskStatus.Blocked    => "blocked",
        _                                  => "pending",
    };
}
