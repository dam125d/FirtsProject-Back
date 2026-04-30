using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.TiposProyectos.DTOs;
using Intap.FirstProject.Application.UseCases.TiposProyectos.Errors;
using Intap.FirstProject.Domain.TiposProyectos;

namespace Intap.FirstProject.Application.UseCases.TiposProyectos.Commands.ChangeTipoProyectoEstado;

internal sealed class ChangeTipoProyectoEstadoCommandHandler(
    ITipoProyectoReadRepository  readRepository,
    ITipoProyectoWriteRepository writeRepository,
    IUnitOfWork                  unitOfWork
) : ICommandHandler<ChangeTipoProyectoEstadoCommand, TipoProyectoEstadoDto>
{
    public async Task<Result<TipoProyectoEstadoDto>> Handle(
        ChangeTipoProyectoEstadoCommand command,
        CancellationToken               ct)
    {
        TipoProyecto? tipoProyecto = await writeRepository.GetByIdAsync(command.Id, ct);
        if (tipoProyecto is null)
            return Result.Failure<TipoProyectoEstadoDto>(TipoProyectoErrors.NotFound);

        // Idempotent: if already in desired state, return current state without persisting
        if (tipoProyecto.Estado == command.Estado)
        {
            return Result.Success(new TipoProyectoEstadoDto(
                tipoProyecto.Id,
                tipoProyecto.Estado,
                tipoProyecto.UpdatedAt ?? tipoProyecto.CreatedAt));
        }

        if (command.Estado)
        {
            // Reactivation: check no other active record has the same name
            bool nombreTaken = await readRepository.ExistsActiveByNombreExcludingIdAsync(
                tipoProyecto.Nombre, command.Id, ct);
            if (nombreTaken)
                return Result.Failure<TipoProyectoEstadoDto>(TipoProyectoErrors.AlreadyExists);

            tipoProyecto.Reactivate();
        }
        else
        {
            tipoProyecto.Deactivate();
        }

        await unitOfWork.CompleteAsync(ct);

        return Result.Success(new TipoProyectoEstadoDto(
            tipoProyecto.Id,
            tipoProyecto.Estado,
            tipoProyecto.UpdatedAt!.Value));
    }
}
