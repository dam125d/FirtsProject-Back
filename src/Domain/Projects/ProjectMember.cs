using Intap.FirstProject.Domain.Users;

namespace Intap.FirstProject.Domain.Projects;

public sealed class ProjectMember
{
    private ProjectMember() { }

    public Guid         Id          { get; private set; } = Guid.NewGuid();
    public Guid         ProjectId   { get; private set; }
    public Guid         UserId      { get; private set; }
    public AccessLevel  AccessLevel { get; private set; }

    // Navigation
    public User User { get; private set; } = null!;

    public static ProjectMember Create(Guid projectId, Guid userId, AccessLevel accessLevel)
    {
        return new ProjectMember
        {
            ProjectId   = projectId,
            UserId      = userId,
            AccessLevel = accessLevel,
        };
    }
}
