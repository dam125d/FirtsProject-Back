using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.Categorias.Errors;
using Intap.FirstProject.Domain.Categorias;

namespace Intap.FirstProject.Application.UseCases.Categorias.Commands.UpdateCategoria;

internal sealed class UpdateCategoriaCommandHandler(
    ICategoriaReadRepository  readRepository,
    ICategoriaWriteRepository writeRepository,
    IUnitOfWork               unitOfWork
) : ICommandHandler<UpdateCategoriaCommand>
{
    public async Task<Result> Handle(UpdateCategoriaCommand command, CancellationToken ct)
    {
        Categoria? categoria = await writeRepository.GetByIdAsync(command.Id, ct);
        if (categoria is null)
            return Result.Failure(CategoriaErrors.NotFound);

        bool nombreTaken = await readRepository.ExistsByNombreExcludingIdAsync(
            command.Nombre.Trim(), command.Id, ct);
        if (nombreTaken)
            return Result.Failure(CategoriaErrors.NombreDuplicado);

        categoria.Update(command.Nombre, command.Descripcion, command.Estado);
        await unitOfWork.CompleteAsync(ct);

        return Result.Success();
    }
}
