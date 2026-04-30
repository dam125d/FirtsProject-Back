using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.TiposTareas.DTOs;
using Intap.FirstProject.Application.UseCases.TiposTareas.Errors;
using Intap.FirstProject.Domain.TiposTareas;

namespace Intap.FirstProject.Application.UseCases.TiposTareas.Commands.ChangeTipoTareaEstado;

internal sealed class ChangeTipoTareaEstadoCommandHandler(
    ITipoTareaReadRepository  readRepository,
    ITipoTareaWriteRepository writeRepository,
    IUnitOfWork               unitOfWork
) : ICommandHandler<ChangeTipoTareaEstadoCommand, TipoTareaEstadoDto>
{
    public async Task<Result<TipoTareaEstadoDto>> Handle(
        ChangeTipoTareaEstadoCommand command,
        CancellationToken            ct)
    {
        TipoTarea? tipoTarea = await writeRepository.GetByIdAsync(command.Id, ct);
        if (tipoTarea is null)
            return Result.Failure<TipoTareaEstadoDto>(TipoTareaErrors.NotFound);

        // Idempotent: if already in desired state, return current state without persisting
        if (tipoTarea.Estado == command.Estado)
        {
            return Result.Success(new TipoTareaEstadoDto(
                tipoTarea.Id,
                tipoTarea.Estado,
                tipoTarea.UpdatedAt ?? tipoTarea.CreatedAt));
        }

        if (command.Estado)
        {
            // Reactivation: verify no other active record has the same name
            bool nombreTaken = await readRepository.ExistsActiveByNombreExcludingIdAsync(
                tipoTarea.Nombre, command.Id, ct);
            if (nombreTaken)
                return Result.Failure<TipoTareaEstadoDto>(TipoTareaErrors.AlreadyExists);

            tipoTarea.Reactivate();
        }
        else
        {
            tipoTarea.Deactivate();
        }

        await unitOfWork.CompleteAsync(ct);

        return Result.Success(new TipoTareaEstadoDto(
            tipoTarea.Id,
            tipoTarea.Estado,
            tipoTarea.UpdatedAt!.Value));
    }
}
