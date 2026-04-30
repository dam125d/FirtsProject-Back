using AutoMapper;
using Intap.FirstProject.Domain.Categorias;

namespace Intap.FirstProject.Application.UseCases.Categorias.DTOs;

public sealed class CategoriaMappingProfile : Profile
{
    public CategoriaMappingProfile()
    {
        CreateMap<Categoria, CategoriaDto>()
            .ConvertUsing((s, _, _) => new CategoriaDto(
                s.Id,
                s.Nombre,
                s.Descripcion,
                s.Estado,
                s.CreatedAt,
                s.UpdatedAt));
    }
}
