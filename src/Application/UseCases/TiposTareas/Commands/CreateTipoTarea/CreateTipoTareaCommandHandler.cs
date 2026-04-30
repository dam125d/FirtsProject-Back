using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.TiposTareas.Errors;
using Intap.FirstProject.Domain.TiposTareas;

namespace Intap.FirstProject.Application.UseCases.TiposTareas.Commands.CreateTipoTarea;

internal sealed class CreateTipoTareaCommandHandler(
    ITipoTareaReadRepository  readRepository,
    ITipoTareaWriteRepository writeRepository,
    IUnitOfWork               unitOfWork
) : ICommandHandler<CreateTipoTareaCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateTipoTareaCommand command, CancellationToken ct)
    {
        bool nombreTaken = await readRepository.ExistsActiveByNombreAsync(command.Nombre.Trim(), ct);
        if (nombreTaken)
            return Result.Failure<Guid>(TipoTareaErrors.AlreadyExists);

        TipoTarea tipoTarea = TipoTarea.Create(command.Nombre, command.Descripcion);

        writeRepository.Add(tipoTarea);
        await unitOfWork.CompleteAsync(ct);

        return Result.Success(tipoTarea.Id);
    }
}
