using Intap.FirstProject.Application.Abstractions.Results;

namespace Intap.FirstProject.Application.UseCases.Projects.Errors;

public static class ProjectErrors
{
    public static readonly ErrorResult NotFound =
        new("Project.NotFound", "Project was not found.", ErrorTypeResult.NotFound);

    public static readonly ErrorResult MemberNotFound =
        new("Project.MemberNotFound", "Project member was not found.", ErrorTypeResult.NotFound);

    public static readonly ErrorResult InvalidAccessLevel =
        new("Project.InvalidAccessLevel", "The provided access level is not valid.", ErrorTypeResult.Validation);
}
