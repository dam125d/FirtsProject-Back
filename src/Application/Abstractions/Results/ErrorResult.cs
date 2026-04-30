namespace Intap.FirstProject.Application.Abstractions.Results;

public sealed record ErrorResult(string Code, string Description, ErrorTypeResult Type)
{
    public static readonly ErrorResult None = new(string.Empty, string.Empty, ErrorTypeResult.Failure);
}
