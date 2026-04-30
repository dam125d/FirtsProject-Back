using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.Empleados.Errors;
using Intap.FirstProject.Domain.Empleados;

namespace Intap.FirstProject.Application.UseCases.Empleados.Commands.UpdateEmpleado;

internal sealed class UpdateEmpleadoCommandHandler(
    IEmpleadoReadRepository  readRepository,
    IEmpleadoWriteRepository writeRepository,
    IUnitOfWork               unitOfWork
) : ICommandHandler<UpdateEmpleadoCommand>
{
    public async Task<Result> Handle(UpdateEmpleadoCommand command, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(command.Identificacion))
            return Result.Failure(EmpleadoErrors.IdentificacionRequerida);

        if (string.IsNullOrWhiteSpace(command.Nombres))
            return Result.Failure(EmpleadoErrors.NombresRequeridos);

        if (string.IsNullOrWhiteSpace(command.Apellidos))
            return Result.Failure(EmpleadoErrors.ApellidosRequeridos);

        if (string.IsNullOrWhiteSpace(command.Correo))
            return Result.Failure(EmpleadoErrors.CorreoRequerido);

        if (string.IsNullOrWhiteSpace(command.Cargo))
            return Result.Failure(EmpleadoErrors.CargoRequerido);

        Empleado? empleado = await writeRepository.GetByIdAsync(command.Id, ct);
        if (empleado is null)
            return Result.Failure(EmpleadoErrors.NotFound);

        bool identificacionTaken = await readRepository.ExistsByIdentificacionExcludingIdAsync(
            command.Identificacion.Trim(), command.Id, ct);
        if (identificacionTaken)
            return Result.Failure(EmpleadoErrors.IdentificacionDuplicada);

        bool correoTaken = await readRepository.ExistsByCorreoExcludingIdAsync(
            command.Correo.Trim(), command.Id, ct);
        if (correoTaken)
            return Result.Failure(EmpleadoErrors.CorreoDuplicado);

        empleado.Update(
            command.Identificacion,
            command.Nombres,
            command.Apellidos,
            command.Correo,
            command.Telefono,
            command.Cargo);

        await unitOfWork.CompleteAsync(ct);

        return Result.Success();
    }
}
