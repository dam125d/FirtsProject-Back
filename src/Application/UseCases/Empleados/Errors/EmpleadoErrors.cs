using Intap.FirstProject.Application.Abstractions.Results;

namespace Intap.FirstProject.Application.UseCases.Empleados.Errors;

public static class EmpleadoErrors
{
    public static readonly ErrorResult NotFound =
        new("Empleado.NotFound", "Employee was not found.", ErrorTypeResult.NotFound);

    public static readonly ErrorResult IdentificacionRequerida =
        new("Empleado.IdentificacionRequerida", "Identification is required.", ErrorTypeResult.Validation);

    public static readonly ErrorResult IdentificacionDuplicada =
        new("Empleado.IdentificacionDuplicada", "Identification already exists.", ErrorTypeResult.Conflict);

    public static readonly ErrorResult NombresRequeridos =
        new("Empleado.NombresRequeridos", "Names are required.", ErrorTypeResult.Validation);

    public static readonly ErrorResult ApellidosRequeridos =
        new("Empleado.ApellidosRequeridos", "Last names are required.", ErrorTypeResult.Validation);

    public static readonly ErrorResult CorreoRequerido =
        new("Empleado.CorreoRequerido", "Email is required.", ErrorTypeResult.Validation);

    public static readonly ErrorResult CorreoInvalido =
        new("Empleado.CorreoInvalido", "Invalid email format.", ErrorTypeResult.Validation);

    public static readonly ErrorResult CorreoDuplicado =
        new("Empleado.CorreoDuplicado", "Email already exists.", ErrorTypeResult.Conflict);

    public static readonly ErrorResult CargoRequerido =
        new("Empleado.CargoRequerido", "Job title is required.", ErrorTypeResult.Validation);
}
