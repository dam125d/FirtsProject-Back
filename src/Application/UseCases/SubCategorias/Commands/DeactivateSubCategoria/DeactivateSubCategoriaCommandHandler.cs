using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Application.UseCases.SubCategorias.Errors;
using Intap.FirstProject.Domain.SubCategorias;
using Microsoft.Extensions.Logging;

namespace Intap.FirstProject.Application.UseCases.SubCategorias.Commands.DeactivateSubCategoria;

internal sealed class DeactivateSubCategoriaCommandHandler(
    ISubCategoriaWriteRepository writeRepository,
    IUnitOfWork                  unitOfWork,
    ILogger<DeactivateSubCategoriaCommandHandler> logger
) : ICommandHandler<DeactivateSubCategoriaCommand>
{
    public async Task<Result> Handle(DeactivateSubCategoriaCommand command, CancellationToken ct)
    {
        SubCategoria? subCategoria = await writeRepository.GetByIdAsync(command.Id, ct);
        if (subCategoria is null)
            return Result.Failure(SubCategoriaErrors.NotFound);

        // Idempotent: already inactive — return success without DB write
        if (!subCategoria.Estado)
            return Result.Success();

        subCategoria.Deactivate();
        await unitOfWork.CompleteAsync(ct);

        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation(
                "SubCategoria deactivated: Id={SubCategoriaId}, CategoriaId={CategoriaId}.",
                subCategoria.Id,
                subCategoria.CategoriaId);

        return Result.Success();
    }
}
