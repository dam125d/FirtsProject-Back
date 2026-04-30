using AutoMapper;
using Intap.FirstProject.Domain.Users;

namespace Intap.FirstProject.Application.UseCases.Users.DTOs;

public sealed class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserDto>();
    }
}
