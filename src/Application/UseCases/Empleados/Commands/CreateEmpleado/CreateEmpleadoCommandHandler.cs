using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.Contracts;
using Intap.FirstProject.Application.Contracts.ReadRepositories;
using Intap.FirstProject.Application.UseCases.Empleados.DTOs;
using Intap.FirstProject.Application.UseCases.Empleados.Errors;
using Intap.FirstProject.Domain.Empleados;

namespace Intap.FirstProject.Application.UseCases.Empleados.Commands.CreateEmpleado;

internal sealed class CreateEmpleadoCommandHandler(
    IEmpleadoReadRepository  readRepository,
    IEmpleadoWriteRepository writeRepository,
    IUnitOfWork               unitOfWork
) : ICommandHandler<CreateEmpleadoCommand, int>
{
    public async Task<Result<int>> Handle(CreateEmpleadoCommand command, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(command.Identificacion))
            return Result.Failure<int>(EmpleadoErrors.IdentificacionRequerida);

        if (string.IsNullOrWhiteSpace(command.Nombres))
            return Result.Failure<int>(EmpleadoErrors.NombresRequeridos);

        if (string.IsNullOrWhiteSpace(command.Apellidos))
            return Result.Failure<int>(EmpleadoErrors.ApellidosRequeridos);

        if (string.IsNullOrWhiteSpace(command.Correo))
            return Result.Failure<int>(EmpleadoErrors.CorreoRequerido);

        if (string.IsNullOrWhiteSpace(command.Cargo))
            return Result.Failure<int>(EmpleadoErrors.CargoRequerido);

        bool identificacionExists = await readRepository.ExistsByIdentificacionAsync(
            command.Identificacion.Trim(), ct);
        if (identificacionExists)
            return Result.Failure<int>(EmpleadoErrors.IdentificacionDuplicada);

        bool correoExists = await readRepository.ExistsByCorreoAsync(
            command.Correo.Trim(), ct);
        if (correoExists)
            return Result.Failure<int>(EmpleadoErrors.CorreoDuplicado);

        Empleado empleado = Empleado.Create(
            command.Identificacion,
            command.Nombres,
            command.Apellidos,
            command.Correo,
            command.Telefono,
            command.Cargo);

        writeRepository.Add(empleado);
        await unitOfWork.CompleteAsync(ct);

        return Result.Success(empleado.Id);
    }
}
