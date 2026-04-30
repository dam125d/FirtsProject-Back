using Intap.FirstProject.Application.Abstractions.Messaging;

namespace Intap.FirstProject.Application.UseCases.Categorias.Commands.UpdateCategoria;

public sealed record UpdateCategoriaCommand(Guid Id, string Nombre, string? Descripcion, bool Estado) : ICommand;
