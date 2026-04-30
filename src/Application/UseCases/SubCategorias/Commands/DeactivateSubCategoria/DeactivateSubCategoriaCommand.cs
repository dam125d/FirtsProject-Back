using Intap.FirstProject.Application.Abstractions.Messaging;

namespace Intap.FirstProject.Application.UseCases.SubCategorias.Commands.DeactivateSubCategoria;

public sealed record DeactivateSubCategoriaCommand(Guid Id) : ICommand;
