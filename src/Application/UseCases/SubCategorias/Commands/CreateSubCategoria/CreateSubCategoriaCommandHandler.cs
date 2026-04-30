using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.SubCategorias.Errors;
using Intap.FirstProject.Domain.Categorias;
using Intap.FirstProject.Domain.SubCategorias;
using Microsoft.Extensions.Logging;

namespace Intap.FirstProject.Application.UseCases.SubCategorias.Commands.CreateSubCategoria;

internal sealed class CreateSubCategoriaCommandHandler(
    ISubCategoriaReadRepository  subCategoriaReadRepository,
    ISubCategoriaWriteRepository subCategoriaWriteRepository,
    ICategoriaReadRepository     categoriaReadRepository,
    IUnitOfWork                  unitOfWork,
    ILogger<CreateSubCategoriaCommandHandler> logger
) : ICommandHandler<CreateSubCategoriaCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateSubCategoriaCommand command, CancellationToken ct)
    {
        string nombreTrimmed = command.Nombre.Trim();

        if (string.IsNullOrEmpty(nombreTrimmed))
            return Result.Failure<Guid>(SubCategoriaErrors.NombreRequerido);

        if (command.CategoriaId == Guid.Empty)
            return Result.Failure<Guid>(SubCategoriaErrors.CategoriaRequerida);

        Categoria? categoria = await categoriaReadRepository.GetByIdAsync(command.CategoriaId, ct);
        if (categoria is null)
            return Result.Failure<Guid>(SubCategoriaErrors.CategoriaNaoEncontrada);

        if (!categoria.Estado)
        {
            if (logger.IsEnabled(LogLevel.Warning))
                logger.LogWarning(
                    "Create SubCategoria blocked — parent category {CategoriaId} is inactive.",
                    command.CategoriaId);
            return Result.Failure<Guid>(SubCategoriaErrors.CategoriaInactiva);
        }

        bool nombreExists = await subCategoriaReadRepository.ExistsByNombreAndCategoriaAsync(
            nombreTrimmed, command.CategoriaId, ct);
        if (nombreExists)
            return Result.Failure<Guid>(SubCategoriaErrors.NombreDuplicado);

        SubCategoria subCategoria = SubCategoria.Create(nombreTrimmed, command.Descripcion, command.CategoriaId);

        subCategoriaWriteRepository.Add(subCategoria);
        await unitOfWork.CompleteAsync(ct);

        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation(
                "SubCategoria created: Id={SubCategoriaId}, CategoriaId={CategoriaId}.",
                subCategoria.Id,
                subCategoria.CategoriaId);

        return Result.Success(subCategoria.Id);
    }
}
