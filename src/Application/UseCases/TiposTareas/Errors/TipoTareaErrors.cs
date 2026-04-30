using Intap.FirstProject.Application.Abstractions.Results;

namespace Intap.FirstProject.Application.UseCases.TiposTareas.Errors;

public static class TipoTareaErrors
{
    public static readonly ErrorResult NotFound =
        new("TIPOS_TAREAS_NOT_FOUND", "No se encontró el tipo de tarea.", ErrorTypeResult.NotFound);

    public static readonly ErrorResult AlreadyExists =
        new("TIPOS_TAREAS_ALREADY_EXISTS", "Ya existe un tipo de tarea activo con ese nombre.", ErrorTypeResult.Conflict);

    public static readonly ErrorResult ValidationError =
        new("TIPOS_TAREAS_VALIDATION_ERROR", "Los datos enviados no son válidos.", ErrorTypeResult.Validation);
}
