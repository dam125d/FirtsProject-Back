using Intap.FirstProject.Application.Abstractions.Results;

namespace Intap.FirstProject.Application.UseCases.TiposProyectos.Errors;

public static class TipoProyectoErrors
{
    public static readonly ErrorResult NotFound =
        new("TIPOS_PROYECTOS_NOT_FOUND", "No se encontró el tipo de proyecto.", ErrorTypeResult.NotFound);

    public static readonly ErrorResult AlreadyExists =
        new("TIPOS_PROYECTOS_ALREADY_EXISTS", "Ya existe un tipo de proyecto activo con ese nombre.", ErrorTypeResult.Conflict);

    public static readonly ErrorResult ValidationError =
        new("TIPOS_PROYECTOS_VALIDATION_ERROR", "Los datos enviados no son válidos.", ErrorTypeResult.Validation);
}
