using FluentAssertions;
using Intap.FirstProject.Application.Abstractions.Pagination;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.UseCases.TiposProyectos.DTOs;
using Intap.FirstProject.Application.UseCases.TiposProyectos.Queries.GetTiposProyectos;
using Intap.FirstProject.Domain.TiposProyectos;
using Intap.FirstProject.Infrastructure.Persistence;
using Intap.FirstProject.Infrastructure.Persistence.Repositories;
using Intap.FirstProject.Tests.Helpers;
using Moq;

namespace Intap.FirstProject.Tests.Application.TiposProyectos;

public sealed class GetTiposProyectosQueryHandlerTests
{
    private static GetTiposProyectosQueryHandler CreateHandler(AppDbContext context)
    {
        Mock<AutoMapper.IMapper> mapper = new();
        mapper.Setup(m => m.Map<List<TipoProyectoDto>>(It.IsAny<List<TipoProyecto>>()))
              .Returns((List<TipoProyecto> items) =>
                  items.Select(t => new TipoProyectoDto(t.Id, t.Nombre, t.Descripcion, t.Estado, t.CreatedAt, t.UpdatedAt)).ToList());
        return new GetTiposProyectosQueryHandler(new TipoProyectoReadRepository(context), mapper.Object);
    }

    // Happy path: returns paged results
    [Fact]
    public async Task Handle_ReturnsPagedResults()
    {
        AppDbContext context = TestDbContextFactory.Create();
        context.TiposProyecto.AddRange(
            TipoProyecto.Create("Alfa", null),
            TipoProyecto.Create("Beta", null),
            TipoProyecto.Create("Gamma", null));
        await context.SaveChangesAsync();

        GetTiposProyectosQueryHandler handler = CreateHandler(context);
        Result<PagedResult<TipoProyectoDto>> result = await handler.Handle(
            new GetTiposProyectosQuery(1, 20, null, null),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(3);
        result.Value.TotalCount.Should().Be(3);
    }

    // Validation: pageSize > 100 returns error
    [Fact]
    public async Task Handle_WithPageSizeAbove100_ReturnsValidationError()
    {
        AppDbContext context = TestDbContextFactory.Create();
        GetTiposProyectosQueryHandler handler = CreateHandler(context);

        Result<PagedResult<TipoProyectoDto>> result = await handler.Handle(
            new GetTiposProyectosQuery(1, 101, null, null),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorTypeResult.Validation);
        result.Error.Code.Should().Be("TIPOS_PROYECTOS_VALIDATION_ERROR");
    }

    // Validation: page < 1 returns error
    [Fact]
    public async Task Handle_WithPageBelowOne_ReturnsValidationError()
    {
        AppDbContext context = TestDbContextFactory.Create();
        GetTiposProyectosQueryHandler handler = CreateHandler(context);

        Result<PagedResult<TipoProyectoDto>> result = await handler.Handle(
            new GetTiposProyectosQuery(0, 20, null, null),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorTypeResult.Validation);
    }

    // Filter by estado=true returns only active records
    [Fact]
    public async Task Handle_FilterByEstadoTrue_ReturnsOnlyActive()
    {
        AppDbContext context = TestDbContextFactory.Create();
        TipoProyecto active   = TipoProyecto.Create("Activo", null);
        TipoProyecto inactive = TipoProyecto.Create("Inactivo", null);
        inactive.Deactivate();
        context.TiposProyecto.AddRange(active, inactive);
        await context.SaveChangesAsync();

        GetTiposProyectosQueryHandler handler = CreateHandler(context);
        Result<PagedResult<TipoProyectoDto>> result = await handler.Handle(
            new GetTiposProyectosQuery(1, 20, true, null),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(1);
        result.Value.Items[0].Nombre.Should().Be("Activo");
    }
}
