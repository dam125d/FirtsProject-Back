using AutoMapper;
using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.Auth.Errors;
using Intap.FirstProject.Application.UseCases.Users.DTOs;
using Intap.FirstProject.Domain.Users;

namespace Intap.FirstProject.Application.UseCases.Users.Queries.GetUserById;

internal sealed class GetUserByIdQueryHandler(
    IUserReadRepository userReadRepository,
    IMapper             mapper
) : IQueryHandler<GetUserByIdQuery, UserDto>
{
    public async Task<Result<UserDto>> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        User? user = await userReadRepository.GetByIdAsync(query.Id, cancellationToken);

        if (user is null)
            return Result.Failure<UserDto>(UserErrors.NotFound);

        return Result.Success(mapper.Map<UserDto>(user));
    }
}
