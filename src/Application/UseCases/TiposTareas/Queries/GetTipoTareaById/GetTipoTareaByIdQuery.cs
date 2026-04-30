using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.UseCases.TiposTareas.DTOs;

namespace Intap.FirstProject.Application.UseCases.TiposTareas.Queries.GetTipoTareaById;

public sealed record GetTipoTareaByIdQuery(Guid Id) : IQuery<TipoTareaDto>;
