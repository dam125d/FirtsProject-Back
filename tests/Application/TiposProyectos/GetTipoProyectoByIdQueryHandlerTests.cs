using FluentAssertions;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.UseCases.TiposProyectos.DTOs;
using Intap.FirstProject.Application.UseCases.TiposProyectos.Queries.GetTipoProyectoById;
using Intap.FirstProject.Domain.TiposProyectos;
using Intap.FirstProject.Infrastructure.Persistence;
using Intap.FirstProject.Infrastructure.Persistence.Repositories;
using Intap.FirstProject.Tests.Helpers;
using Moq;

namespace Intap.FirstProject.Tests.Application.TiposProyectos;

public sealed class GetTipoProyectoByIdQueryHandlerTests
{
    private static GetTipoProyectoByIdQueryHandler CreateHandler(AppDbContext context)
    {
        Mock<AutoMapper.IMapper> mapper = new();
        mapper.Setup(m => m.Map<TipoProyectoDto>(It.IsAny<TipoProyecto>()))
              .Returns((TipoProyecto t) => new TipoProyectoDto(t.Id, t.Nombre, t.Descripcion, t.Estado, t.CreatedAt, t.UpdatedAt));
        return new GetTipoProyectoByIdQueryHandler(new TipoProyectoReadRepository(context), mapper.Object);
    }

    // Happy path: returns dto for existing id
    [Fact]
    public async Task Handle_WithExistingId_ReturnsTipoProyectoDto()
    {
        AppDbContext context = TestDbContextFactory.Create();
        TipoProyecto t = TipoProyecto.Create("Desarrollo", "Desc");
        context.TiposProyecto.Add(t);
        await context.SaveChangesAsync();

        GetTipoProyectoByIdQueryHandler handler = CreateHandler(context);
        Result<TipoProyectoDto> result = await handler.Handle(
            new GetTipoProyectoByIdQuery(t.Id),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(t.Id);
        result.Value.Nombre.Should().Be("Desarrollo");
        result.Value.Estado.Should().BeTrue();
    }

    // Edge case: ID not found returns 404
    [Fact]
    public async Task Handle_WithNonExistingId_ReturnsNotFound()
    {
        AppDbContext context = TestDbContextFactory.Create();
        GetTipoProyectoByIdQueryHandler handler = CreateHandler(context);

        Result<TipoProyectoDto> result = await handler.Handle(
            new GetTipoProyectoByIdQuery(Guid.NewGuid()),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorTypeResult.NotFound);
        result.Error.Code.Should().Be("TIPOS_PROYECTOS_NOT_FOUND");
    }
}
