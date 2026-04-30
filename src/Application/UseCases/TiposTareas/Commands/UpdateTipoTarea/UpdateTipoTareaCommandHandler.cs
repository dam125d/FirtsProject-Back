using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.TiposTareas.Errors;
using Intap.FirstProject.Domain.TiposTareas;

namespace Intap.FirstProject.Application.UseCases.TiposTareas.Commands.UpdateTipoTarea;

internal sealed class UpdateTipoTareaCommandHandler(
    ITipoTareaReadRepository  readRepository,
    ITipoTareaWriteRepository writeRepository,
    IUnitOfWork               unitOfWork
) : ICommandHandler<UpdateTipoTareaCommand, Guid>
{
    public async Task<Result<Guid>> Handle(UpdateTipoTareaCommand command, CancellationToken ct)
    {
        TipoTarea? tipoTarea = await writeRepository.GetByIdAsync(command.Id, ct);
        if (tipoTarea is null)
            return Result.Failure<Guid>(TipoTareaErrors.NotFound);

        bool nombreTaken = await readRepository.ExistsActiveByNombreExcludingIdAsync(
            command.Nombre.Trim(), command.Id, ct);
        if (nombreTaken)
            return Result.Failure<Guid>(TipoTareaErrors.AlreadyExists);

        tipoTarea.Update(command.Nombre, command.Descripcion);
        await unitOfWork.CompleteAsync(ct);

        return Result.Success(tipoTarea.Id);
    }
}
