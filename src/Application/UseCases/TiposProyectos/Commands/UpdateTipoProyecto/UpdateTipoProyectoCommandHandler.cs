using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.TiposProyectos.Errors;
using Intap.FirstProject.Domain.TiposProyectos;

namespace Intap.FirstProject.Application.UseCases.TiposProyectos.Commands.UpdateTipoProyecto;

internal sealed class UpdateTipoProyectoCommandHandler(
    ITipoProyectoReadRepository  readRepository,
    ITipoProyectoWriteRepository writeRepository,
    IUnitOfWork                  unitOfWork
) : ICommandHandler<UpdateTipoProyectoCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        UpdateTipoProyectoCommand command,
        CancellationToken         ct)
    {
        TipoProyecto? tipoProyecto = await writeRepository.GetByIdAsync(command.Id, ct);
        if (tipoProyecto is null)
            return Result.Failure<Guid>(TipoProyectoErrors.NotFound);

        bool nombreTaken = await readRepository.ExistsActiveByNombreExcludingIdAsync(
            command.Nombre.Trim(), command.Id, ct);
        if (nombreTaken)
            return Result.Failure<Guid>(TipoProyectoErrors.AlreadyExists);

        tipoProyecto.Update(command.Nombre, command.Descripcion);
        await unitOfWork.CompleteAsync(ct);

        return Result.Success(tipoProyecto.Id);
    }
}
