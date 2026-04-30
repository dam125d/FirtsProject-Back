using FluentAssertions;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.UseCases.Categorias.DTOs;
using Intap.FirstProject.Application.UseCases.Categorias.Queries.GetCategorias;
using Intap.FirstProject.Domain.Categorias;
using Intap.FirstProject.Infrastructure.Persistence;
using Intap.FirstProject.Infrastructure.Persistence.Repositories;
using Intap.FirstProject.Tests.Helpers;
using Moq;

namespace Intap.FirstProject.Tests.Application.Categorias;

public sealed class GetCategoriasQueryHandlerTests
{
    private static GetCategoriasQueryHandler CreateHandler(AppDbContext context)
    {
        Mock<AutoMapper.IMapper> mapper = new();
        mapper.Setup(m => m.Map<List<CategoriaDto>>(It.IsAny<List<Categoria>>()))
              .Returns((List<Categoria> cats) =>
                  cats.Select(c => new CategoriaDto(c.Id, c.Nombre, c.Descripcion, c.Estado, c.CreatedAt, c.UpdatedAt)).ToList());

        return new GetCategoriasQueryHandler(new CategoriaReadRepository(context), mapper.Object);
    }

    // IT-01: Listar categorías activas (default soloActivas=true)
    [Fact]
    public async Task Handle_SoloActivas_ReturnsSortedActiveOnly()
    {
        // Arrange
        AppDbContext context = TestDbContextFactory.Create();

        Categoria active1  = Categoria.Create("Tecnología",    null);
        Categoria active2  = Categoria.Create("Administración", null);
        Categoria inactive = Categoria.Create("Archivado",     null);
        inactive.Deactivate();

        context.Categorias.AddRange(active1, active2, inactive);
        await context.SaveChangesAsync();

        GetCategoriasQueryHandler handler = CreateHandler(context);

        // Act
        Result<List<CategoriaDto>> result = await handler.Handle(new GetCategoriasQuery(true), CancellationToken.None);

        // Assert — IT-01: 2 activas, ordenadas por nombre ASC
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value[0].Nombre.Should().Be("Administración");
        result.Value[1].Nombre.Should().Be("Tecnología");
        result.Value.Should().OnlyContain(c => c.Estado);
    }

    // IT-02: Listar todas incluyendo inactivas
    [Fact]
    public async Task Handle_SoloActivasFalse_ReturnsAll()
    {
        // Arrange
        AppDbContext context = TestDbContextFactory.Create();

        Categoria active   = Categoria.Create("Activa",   null);
        Categoria inactive = Categoria.Create("Inactiva", null);
        inactive.Deactivate();

        context.Categorias.AddRange(active, inactive);
        await context.SaveChangesAsync();

        GetCategoriasQueryHandler handler = CreateHandler(context);

        // Act
        Result<List<CategoriaDto>> result = await handler.Handle(new GetCategoriasQuery(false), CancellationToken.None);

        // Assert — IT-02: todas incluyendo inactivas
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_EmptyDatabase_ReturnsEmptyListWithSuccess()
    {
        // Arrange
        AppDbContext context = TestDbContextFactory.Create();
        GetCategoriasQueryHandler handler = CreateHandler(context);

        // Act
        Result<List<CategoriaDto>> result = await handler.Handle(new GetCategoriasQuery(true), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
