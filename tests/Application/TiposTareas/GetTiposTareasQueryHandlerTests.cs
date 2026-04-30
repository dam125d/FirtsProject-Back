using FluentAssertions;
using Intap.FirstProject.Application.Abstractions.Pagination;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.UseCases.TiposTareas.DTOs;
using Intap.FirstProject.Application.UseCases.TiposTareas.Queries.GetTiposTareas;
using Intap.FirstProject.Domain.TiposTareas;
using Intap.FirstProject.Infrastructure.Persistence;
using Intap.FirstProject.Infrastructure.Persistence.Repositories;
using Intap.FirstProject.Tests.Helpers;
using Moq;

namespace Intap.FirstProject.Tests.Application.TiposTareas;

public sealed class GetTiposTareasQueryHandlerTests
{
    private static GetTiposTareasQueryHandler CreateHandler(AppDbContext context)
    {
        Mock<AutoMapper.IMapper> mapper = new();
        mapper.Setup(m => m.Map<List<TipoTareaDto>>(It.IsAny<List<TipoTarea>>()))
              .Returns((List<TipoTarea> items) =>
                  items.Select(t => new TipoTareaDto(t.Id, t.Nombre, t.Descripcion, t.Estado, t.CreatedAt, t.UpdatedAt)).ToList());
        return new(new TipoTareaReadRepository(context), mapper.Object);
    }

    // Happy path: returns paged results
    [Fact]
    public async Task Handle_WithValidQuery_ReturnsPagedResult()
    {
        AppDbContext context = TestDbContextFactory.Create();
        context.TiposTarea.AddRange(
            TipoTarea.Create("Análisis", null),
            TipoTarea.Create("Desarrollo", null),
            TipoTarea.Create("Testing", null));
        await context.SaveChangesAsync();

        GetTiposTareasQueryHandler handler = CreateHandler(context);
        Result<PagedResult<TipoTareaDto>> result = await handler.Handle(
            new GetTiposTareasQuery(1, 10, null, null),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalCount.Should().Be(3);
        result.Value.Items.Should().HaveCount(3);
    }

    // Filter by estado=true returns only active
    [Fact]
    public async Task Handle_FilterByEstadoTrue_ReturnsOnlyActive()
    {
        AppDbContext context = TestDbContextFactory.Create();
        TipoTarea active = TipoTarea.Create("Activo", null);
        TipoTarea inactive = TipoTarea.Create("Inactivo", null);
        inactive.Deactivate();
        context.TiposTarea.AddRange(active, inactive);
        await context.SaveChangesAsync();

        GetTiposTareasQueryHandler handler = CreateHandler(context);
        Result<PagedResult<TipoTareaDto>> result = await handler.Handle(
            new GetTiposTareasQuery(1, 10, true, null),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().AllSatisfy(x => x.Estado.Should().BeTrue());
    }

    // Edge case: invalid page returns ValidationError
    [Fact]
    public async Task Handle_WithInvalidPage_ReturnsValidationError()
    {
        AppDbContext context = TestDbContextFactory.Create();
        GetTiposTareasQueryHandler handler = CreateHandler(context);

        Result<PagedResult<TipoTareaDto>> result = await handler.Handle(
            new GetTiposTareasQuery(0, 10, null, null),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorTypeResult.Validation);
        result.Error.Code.Should().Be("TIPOS_TAREAS_VALIDATION_ERROR");
    }

    // Edge case: pageSize > 100 returns ValidationError
    [Fact]
    public async Task Handle_WithPageSizeOver100_ReturnsValidationError()
    {
        AppDbContext context = TestDbContextFactory.Create();
        GetTiposTareasQueryHandler handler = CreateHandler(context);

        Result<PagedResult<TipoTareaDto>> result = await handler.Handle(
            new GetTiposTareasQuery(1, 101, null, null),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorTypeResult.Validation);
    }
}
