using Intap.FirstProject.Application.Abstractions.Messaging;

namespace Intap.FirstProject.Application.UseCases.SubCategorias.Commands.CreateSubCategoria;

public sealed record CreateSubCategoriaCommand(
    string  Nombre,
    string? Descripcion,
    Guid    CategoriaId
) : ICommand<Guid>;
