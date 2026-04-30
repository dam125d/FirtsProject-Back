using Intap.FirstProject.Application.Abstractions.Messaging;

namespace Intap.FirstProject.Application.UseCases.Categorias.Commands.CreateCategoria;

public sealed record CreateCategoriaCommand(string Nombre, string? Descripcion) : ICommand<Guid>;
