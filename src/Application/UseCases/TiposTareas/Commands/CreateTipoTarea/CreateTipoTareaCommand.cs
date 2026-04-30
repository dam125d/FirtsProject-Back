using Intap.FirstProject.Application.Abstractions.Messaging;

namespace Intap.FirstProject.Application.UseCases.TiposTareas.Commands.CreateTipoTarea;

public sealed record CreateTipoTareaCommand(string Nombre, string? Descripcion) : ICommand<Guid>;
