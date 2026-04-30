using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.TiposProyectos.Errors;
using Intap.FirstProject.Domain.TiposProyectos;

namespace Intap.FirstProject.Application.UseCases.TiposProyectos.Commands.CreateTipoProyecto;

internal sealed class CreateTipoProyectoCommandHandler(
    ITipoProyectoReadRepository  readRepository,
    ITipoProyectoWriteRepository writeRepository,
    IUnitOfWork                  unitOfWork
) : ICommandHandler<CreateTipoProyectoCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateTipoProyectoCommand command,
        CancellationToken         ct)
    {
        bool nombreExists = await readRepository.ExistsActiveByNombreAsync(command.Nombre.Trim(), ct);
        if (nombreExists)
            return Result.Failure<Guid>(TipoProyectoErrors.AlreadyExists);

        TipoProyecto tipoProyecto = TipoProyecto.Create(command.Nombre, command.Descripcion);

        writeRepository.Add(tipoProyecto);
        await unitOfWork.CompleteAsync(ct);

        return Result.Success(tipoProyecto.Id);
    }
}
