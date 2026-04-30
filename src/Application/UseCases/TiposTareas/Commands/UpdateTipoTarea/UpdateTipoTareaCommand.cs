using Intap.FirstProject.Application.Abstractions.Messaging;

namespace Intap.FirstProject.Application.UseCases.TiposTareas.Commands.UpdateTipoTarea;

public sealed record UpdateTipoTareaCommand(Guid Id, string Nombre, string? Descripcion) : ICommand<Guid>;
