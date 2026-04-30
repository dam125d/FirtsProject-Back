using FluentAssertions;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.UseCases.SubCategorias.DTOs;
using Intap.FirstProject.Application.UseCases.SubCategorias.Queries.GetSubCategorias;
using Intap.FirstProject.Domain.Categorias;
using Intap.FirstProject.Domain.SubCategorias;
using Intap.FirstProject.Infrastructure.Persistence;
using Intap.FirstProject.Infrastructure.Persistence.Repositories;
using Intap.FirstProject.Tests.Helpers;
using Moq;

namespace Intap.FirstProject.Tests.Application.SubCategorias;

public sealed class GetSubCategoriasQueryHandlerTests
{
    private static GetSubCategoriasQueryHandler CreateHandler(AppDbContext context)
    {
        Mock<AutoMapper.IMapper> mapper = new();
        mapper.Setup(m => m.Map<List<SubCategoriaDto>>(It.IsAny<List<SubCategoria>>()))
              .Returns((List<SubCategoria> subs) =>
                  subs.Select(s => new SubCategoriaDto(
                      s.Id, s.Nombre, s.Descripcion, s.CategoriaId, s.Estado, s.CreatedAt, s.UpdatedAt)).ToList());

        return new GetSubCategoriasQueryHandler(new SubCategoriaReadRepository(context), mapper.Object);
    }

    private static async Task<Categoria> SeedCategoriaAsync(AppDbContext context, string nombre = "Tecnología")
    {
        Categoria categoria = Categoria.Create(nombre, null);
        context.Categorias.Add(categoria);
        await context.SaveChangesAsync();
        return categoria;
    }

    [Fact]
    public async Task Handle_WithSoloActivasTrue_ReturnsOnlyActiveSubCategorias()
    {
        AppDbContext context = TestDbContextFactory.Create();
        Categoria categoria = await SeedCategoriaAsync(context);

        SubCategoria active   = SubCategoria.Create("Laptops", null, categoria.Id);
        SubCategoria inactive = SubCategoria.Create("Tablets", null, categoria.Id);
        inactive.Deactivate();
        context.SubCategorias.AddRange(active, inactive);
        await context.SaveChangesAsync();

        GetSubCategoriasQueryHandler handler = CreateHandler(context);
        Result<List<SubCategoriaDto>> result = await handler.Handle(new GetSubCategoriasQuery(SoloActivas: true), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value[0].Nombre.Should().Be("Laptops");
    }

    [Fact]
    public async Task Handle_WithSoloActivasFalse_ReturnsAllSubCategorias()
    {
        AppDbContext context = TestDbContextFactory.Create();
        Categoria categoria = await SeedCategoriaAsync(context);

        SubCategoria active   = SubCategoria.Create("Laptops", null, categoria.Id);
        SubCategoria inactive = SubCategoria.Create("Tablets", null, categoria.Id);
        inactive.Deactivate();
        context.SubCategorias.AddRange(active, inactive);
        await context.SaveChangesAsync();

        GetSubCategoriasQueryHandler handler = CreateHandler(context);
        Result<List<SubCategoriaDto>> result = await handler.Handle(new GetSubCategoriasQuery(SoloActivas: false), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WithCategoriaIdFilter_ReturnsOnlyMatchingCategory()
    {
        AppDbContext context = TestDbContextFactory.Create();
        Categoria cat1 = await SeedCategoriaAsync(context, "Tecnología");
        Categoria cat2 = await SeedCategoriaAsync(context, "Electrónica");

        context.SubCategorias.AddRange(
            SubCategoria.Create("Laptops", null, cat1.Id),
            SubCategoria.Create("Móviles", null, cat2.Id));
        await context.SaveChangesAsync();

        GetSubCategoriasQueryHandler handler = CreateHandler(context);
        Result<List<SubCategoriaDto>> result = await handler.Handle(
            new GetSubCategoriasQuery(SoloActivas: true, CategoriaId: cat1.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value[0].Nombre.Should().Be("Laptops");
    }

    [Fact]
    public async Task Handle_WithNoSubCategorias_ReturnsEmptyList()
    {
        AppDbContext context = TestDbContextFactory.Create();
        GetSubCategoriasQueryHandler handler = CreateHandler(context);

        Result<List<SubCategoriaDto>> result = await handler.Handle(new GetSubCategoriasQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ReturnsSubCategoriasOrderedByNombreAsc()
    {
        AppDbContext context = TestDbContextFactory.Create();
        Categoria categoria = await SeedCategoriaAsync(context);

        context.SubCategorias.AddRange(
            SubCategoria.Create("Tablets", null, categoria.Id),
            SubCategoria.Create("Auriculares", null, categoria.Id),
            SubCategoria.Create("Laptops", null, categoria.Id));
        await context.SaveChangesAsync();

        GetSubCategoriasQueryHandler handler = CreateHandler(context);
        Result<List<SubCategoriaDto>> result = await handler.Handle(new GetSubCategoriasQuery(SoloActivas: true), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Select(s => s.Nombre).Should().BeInAscendingOrder();
    }
}
