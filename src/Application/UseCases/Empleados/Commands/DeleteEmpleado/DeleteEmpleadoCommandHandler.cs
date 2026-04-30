using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Application.UseCases.Empleados.Errors;
using Intap.FirstProject.Domain.Empleados;

namespace Intap.FirstProject.Application.UseCases.Empleados.Commands.DeleteEmpleado;

internal sealed class DeleteEmpleadoCommandHandler(
    IEmpleadoWriteRepository writeRepository,
    IUnitOfWork                unitOfWork
) : ICommandHandler<DeleteEmpleadoCommand>
{
    public async Task<Result> Handle(DeleteEmpleadoCommand command, CancellationToken ct)
    {
        Empleado? empleado = await writeRepository.GetByIdAsync(command.Id, ct);
        if (empleado is null)
            return Result.Failure(EmpleadoErrors.NotFound);

        empleado.Delete();
        await unitOfWork.CompleteAsync(ct);

        return Result.Success();
    }
}
