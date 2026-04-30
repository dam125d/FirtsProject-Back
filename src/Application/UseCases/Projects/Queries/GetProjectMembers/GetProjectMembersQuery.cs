using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.UseCases.Projects.DTOs;

namespace Intap.FirstProject.Application.UseCases.Projects.Queries.GetProjectMembers;

public sealed record GetProjectMembersQuery(Guid ProjectId) : IQuery<List<TeamMemberDto>>;
