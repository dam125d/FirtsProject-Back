using Intap.FirstProject.Application.Abstractions.Messaging;

namespace Intap.FirstProject.Application.UseCases.Categorias.Commands.DeactivateCategoria;

public sealed record DeactivateCategoriaCommand(Guid Id) : ICommand;
