using Intap.FirstProject.Application.Abstractions.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intap.FirstProject.API.Common;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public abstract class ApiController : ControllerBase
{
    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess) return NoContent();
        return MapError(result.Error);
    }

    protected IActionResult HandleResult<TValue>(Result<TValue> result)
    {
        if (result.IsSuccess) return Ok(result.Value);
        return MapError(result.Error);
    }

    private IActionResult MapError(ErrorResult error) => error.Type switch
    {
        ErrorTypeResult.NotFound       => NotFound(new { error.Code, error.Description }),
        ErrorTypeResult.Validation     => BadRequest(new { error.Code, error.Description }),
        ErrorTypeResult.Conflict       => Conflict(new { error.Code, error.Description }),
        ErrorTypeResult.Unauthorized   => Unauthorized(new { error.Code, error.Description }),
        ErrorTypeResult.Unprocessable  => UnprocessableEntity(new { error.Code, error.Description }),
        ErrorTypeResult.Problem        => StatusCode(500, new { error.Code, error.Description }),
        _                              => BadRequest(new { error.Code, error.Description }),
    };
}
