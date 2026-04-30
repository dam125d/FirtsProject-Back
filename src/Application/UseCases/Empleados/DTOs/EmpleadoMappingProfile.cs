using AutoMapper;
using Intap.FirstProject.Domain.Empleados;

namespace Intap.FirstProject.Application.UseCases.Empleados.DTOs;

public sealed class EmpleadoMappingProfile : Profile
{
    public EmpleadoMappingProfile()
    {
        CreateMap<Empleado, EmpleadoDto>()
            .ConvertUsing((s, _, _) => new EmpleadoDto(
                s.Id,
                s.Identificacion,
                s.Nombres,
                s.Apellidos,
                s.Correo,
                s.Telefono,
                s.Cargo,
                s.Estado,
                s.CreatedAt,
                s.UpdatedAt));

        CreateMap<Empleado, EmpleadoCreatedDto>()
            .ConvertUsing((s, _, _) => new EmpleadoCreatedDto(
                s.Id,
                s.Identificacion,
                s.Nombres,
                s.Apellidos,
                s.Correo,
                s.Telefono,
                s.Cargo,
                s.CreatedAt,
                s.UpdatedAt));
    }
}
