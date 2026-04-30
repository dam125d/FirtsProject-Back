using FluentAssertions;
using Intap.FirstProject.Application.UseCases.Categorias.Commands.CreateCategoria;

namespace Intap.FirstProject.Tests.Application.Categorias;

public sealed class CreateCategoriaCommandValidatorTests
{
    private readonly CreateCategoriaCommandValidator _validator = new();

    // IT-07: Crear categoría sin nombre → 400
    [Theory]
    [InlineData("",    false)]
    [InlineData("   ", false)]
    [InlineData(null!,  false)]
    public void Validate_EmptyOrNullNombre_IsInvalid(string? nombre, bool expected)
    {
        CreateCategoriaCommand command = new(nombre!, null);
        FluentValidation.Results.ValidationResult result = _validator.Validate(command);
        result.IsValid.Should().Be(expected);
    }

    [Fact]
    public void Validate_NombreExceedsMaxLength_IsInvalid()
    {
        CreateCategoriaCommand command = new(new string('A', 101), null);
        FluentValidation.Results.ValidationResult result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_DescripcionExceedsMaxLength_IsInvalid()
    {
        CreateCategoriaCommand command = new("Nombre válido", new string('X', 501));
        FluentValidation.Results.ValidationResult result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_ValidCommand_IsValid()
    {
        CreateCategoriaCommand command = new("Tecnología", "Proyectos de TI");
        FluentValidation.Results.ValidationResult result = _validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_NullDescripcion_IsValid()
    {
        CreateCategoriaCommand command = new("Tecnología", null);
        FluentValidation.Results.ValidationResult result = _validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }
}
