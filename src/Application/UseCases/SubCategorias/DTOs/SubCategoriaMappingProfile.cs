using AutoMapper;
using Intap.FirstProject.Domain.SubCategorias;

namespace Intap.FirstProject.Application.UseCases.SubCategorias.DTOs;

public sealed class SubCategoriaMappingProfile : Profile
{
    public SubCategoriaMappingProfile()
    {
        CreateMap<SubCategoria, SubCategoriaDto>()
            .ConvertUsing((s, _, _) => new SubCategoriaDto(
                s.Id,
                s.Nombre,
                s.Descripcion,
                s.CategoriaId,
                s.Estado,
                s.CreatedAt,
                s.UpdatedAt));
    }
}
