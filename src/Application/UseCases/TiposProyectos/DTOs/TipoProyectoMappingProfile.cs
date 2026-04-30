using AutoMapper;
using Intap.FirstProject.Domain.TiposProyectos;

namespace Intap.FirstProject.Application.UseCases.TiposProyectos.DTOs;

public sealed class TipoProyectoMappingProfile : Profile
{
    public TipoProyectoMappingProfile()
    {
        CreateMap<TipoProyecto, TipoProyectoDto>()
            .ConvertUsing((s, _, _) => new TipoProyectoDto(
                s.Id,
                s.Nombre,
                s.Descripcion,
                s.Estado,
                s.CreatedAt,
                s.UpdatedAt));
    }
}
