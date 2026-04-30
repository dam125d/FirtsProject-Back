using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.SubCategorias.Errors;
using Intap.FirstProject.Domain.Categorias;
using Intap.FirstProject.Domain.SubCategorias;

namespace Intap.FirstProject.Application.UseCases.SubCategorias.Commands.UpdateSubCategoria;

internal sealed class UpdateSubCategoriaCommandHandler(
    ISubCategoriaReadRepository  subCategoriaReadRepository,
    ISubCategoriaWriteRepository subCategoriaWriteRepository,
    ICategoriaReadRepository     categoriaReadRepository,
    IUnitOfWork                  unitOfWork
) : ICommandHandler<UpdateSubCategoriaCommand>
{
    public async Task<Result> Handle(UpdateSubCategoriaCommand command, CancellationToken ct)
    {
        SubCategoria? subCategoria = await subCategoriaWriteRepository.GetByIdAsync(command.Id, ct);
        if (subCategoria is null)
            return Result.Failure(SubCategoriaErrors.NotFound);

        string nombreTrimmed = command.Nombre.Trim();

        if (string.IsNullOrEmpty(nombreTrimmed))
            return Result.Failure(SubCategoriaErrors.NombreRequerido);

        if (command.CategoriaId == Guid.Empty)
            return Result.Failure(SubCategoriaErrors.CategoriaRequerida);

        Categoria? categoria = await categoriaReadRepository.GetByIdAsync(command.CategoriaId, ct);
        if (categoria is null)
            return Result.Failure(SubCategoriaErrors.CategoriaNaoEncontrada);

        // Validate active state only when CategoriaId changes
        if (command.CategoriaId != subCategoria.CategoriaId && !categoria.Estado)
            return Result.Failure(SubCategoriaErrors.CategoriaInactiva);

        bool nombreTaken = await subCategoriaReadRepository.ExistsByNombreAndCategoriaExcludingIdAsync(
            nombreTrimmed, command.CategoriaId, command.Id, ct);
        if (nombreTaken)
            return Result.Failure(SubCategoriaErrors.NombreDuplicado);

        subCategoria.Update(nombreTrimmed, command.Descripcion, command.CategoriaId, command.Estado);
        await unitOfWork.CompleteAsync(ct);

        return Result.Success();
    }
}
