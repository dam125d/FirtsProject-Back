using FluentAssertions;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.UseCases.Categorias.DTOs;
using Intap.FirstProject.Application.UseCases.Categorias.Queries.GetCategoriaById;
using Intap.FirstProject.Domain.Categorias;
using Intap.FirstProject.Infrastructure.Persistence;
using Intap.FirstProject.Infrastructure.Persistence.Repositories;
using Intap.FirstProject.Tests.Helpers;
using Moq;

namespace Intap.FirstProject.Tests.Application.Categorias;

public sealed class GetCategoriaByIdQueryHandlerTests
{
    private static GetCategoriaByIdQueryHandler CreateHandler(AppDbContext context)
    {
        Mock<AutoMapper.IMapper> mapper = new();
        mapper.Setup(m => m.Map<CategoriaDto>(It.IsAny<Categoria>()))
              .Returns((Categoria c) => new CategoriaDto(c.Id, c.Nombre, c.Descripcion, c.Estado, c.CreatedAt, c.UpdatedAt));

        return new GetCategoriaByIdQueryHandler(new CategoriaReadRepository(context), mapper.Object);
    }

    // IT-03: Obtener categoría por ID existente
    [Fact]
    public async Task Handle_WithExistingId_ReturnsCategoriaDto()
    {
        // Arrange
        AppDbContext context = TestDbContextFactory.Create();

        Categoria categoria = Categoria.Create("Tecnología", "Proyectos de TI");
        context.Categorias.Add(categoria);
        await context.SaveChangesAsync();

        GetCategoriaByIdQueryHandler handler = CreateHandler(context);

        // Act
        Result<CategoriaDto> result = await handler.Handle(new GetCategoriaByIdQuery(categoria.Id), CancellationToken.None);

        // Assert — IT-03: 200 con datos completos
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(categoria.Id);
        result.Value.Nombre.Should().Be("Tecnología");
        result.Value.Descripcion.Should().Be("Proyectos de TI");
        result.Value.Estado.Should().BeTrue();
    }

    // IT-04: Obtener categoría por ID inexistente
    [Fact]
    public async Task Handle_WithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        AppDbContext context = TestDbContextFactory.Create();
        GetCategoriaByIdQueryHandler handler = CreateHandler(context);

        // Act
        Result<CategoriaDto> result = await handler.Handle(new GetCategoriaByIdQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert — IT-04: 404 con error_code CATEGORIA_NOT_FOUND
        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorTypeResult.NotFound);
        result.Error.Code.Should().Be("Categoria.NotFound");
    }
}
