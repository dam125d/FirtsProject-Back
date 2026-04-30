using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.Categorias.Errors;
using Intap.FirstProject.Domain.Categorias;

namespace Intap.FirstProject.Application.UseCases.Categorias.Commands.CreateCategoria;

internal sealed class CreateCategoriaCommandHandler(
    ICategoriaReadRepository  readRepository,
    ICategoriaWriteRepository writeRepository,
    IUnitOfWork               unitOfWork
) : ICommandHandler<CreateCategoriaCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateCategoriaCommand command, CancellationToken ct)
    {
        bool nombreExists = await readRepository.ExistsByNombreAsync(command.Nombre.Trim(), ct);
        if (nombreExists)
            return Result.Failure<Guid>(CategoriaErrors.NombreDuplicado);

        Categoria categoria = Categoria.Create(command.Nombre, command.Descripcion);

        writeRepository.Add(categoria);
        await unitOfWork.CompleteAsync(ct);

        return Result.Success(categoria.Id);
    }
}
