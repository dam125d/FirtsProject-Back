using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.UseCases.Auth.DTOs;

namespace Intap.FirstProject.Application.UseCases.Auth.Queries.GetCurrentUser;

public sealed record GetCurrentUserQuery : IQuery<LoginResponse>;
