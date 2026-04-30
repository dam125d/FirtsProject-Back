using Intap.FirstProject.Application.Abstractions.Messaging;

namespace Intap.FirstProject.Application.UseCases.SubCategorias.Commands.UpdateSubCategoria;

public sealed record UpdateSubCategoriaCommand(
    Guid    Id,
    string  Nombre,
    string? Descripcion,
    Guid    CategoriaId,
    bool    Estado
) : ICommand;
