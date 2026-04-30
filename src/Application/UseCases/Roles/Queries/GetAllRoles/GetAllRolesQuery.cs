using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.UseCases.Roles.DTOs;

namespace Intap.FirstProject.Application.UseCases.Roles.Queries.GetAllRoles;

public sealed record GetAllRolesQuery : IQuery<List<RoleDto>>;
