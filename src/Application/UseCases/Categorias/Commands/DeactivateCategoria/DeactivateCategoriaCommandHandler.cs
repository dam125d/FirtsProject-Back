using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.Categorias.Errors;
using Intap.FirstProject.Domain.Categorias;

namespace Intap.FirstProject.Application.UseCases.Categorias.Commands.DeactivateCategoria;

internal sealed class DeactivateCategoriaCommandHandler(
    ICategoriaReadRepository  readRepository,
    ICategoriaWriteRepository writeRepository,
    IUnitOfWork               unitOfWork
) : ICommandHandler<DeactivateCategoriaCommand>
{
    public async Task<Result> Handle(DeactivateCategoriaCommand command, CancellationToken ct)
    {
        Categoria? categoria = await writeRepository.GetByIdAsync(command.Id, ct);
        if (categoria is null)
            return Result.Failure(CategoriaErrors.NotFound);

        bool hasActiveProjects = await readRepository.HasActiveProjectsAsync(command.Id, ct);
        if (hasActiveProjects)
            return Result.Failure(CategoriaErrors.EnUso);

        categoria.Deactivate();
        await unitOfWork.CompleteAsync(ct);

        return Result.Success();
    }
}
