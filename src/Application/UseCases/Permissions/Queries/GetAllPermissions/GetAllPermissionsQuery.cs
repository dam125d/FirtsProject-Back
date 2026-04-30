using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.UseCases.Permissions.DTOs;

namespace Intap.FirstProject.Application.UseCases.Permissions.Queries.GetAllPermissions;

public sealed record GetAllPermissionsQuery : IQuery<List<PermissionDto>>;
