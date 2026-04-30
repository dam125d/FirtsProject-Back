using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.UseCases.TiposProyectos.DTOs;

namespace Intap.FirstProject.Application.UseCases.TiposProyectos.Queries.GetTipoProyectoById;

public sealed record GetTipoProyectoByIdQuery(Guid Id) : IQuery<TipoProyectoDto>;
