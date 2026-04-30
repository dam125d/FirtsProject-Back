using AutoMapper;
using Intap.FirstProject.Domain.TiposTareas;

namespace Intap.FirstProject.Application.UseCases.TiposTareas.DTOs;

public sealed class TipoTareasProfile : Profile
{
    public TipoTareasProfile()
    {
        CreateMap<TipoTarea, TipoTareaDto>()
            .ConvertUsing((s, _, _) => new TipoTareaDto(
                s.Id,
                s.Nombre,
                s.Descripcion,
                s.Estado,
                s.CreatedAt,
                s.UpdatedAt));
    }
}
