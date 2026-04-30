using FluentAssertions;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.UseCases.TiposTareas.DTOs;
using Intap.FirstProject.Application.UseCases.TiposTareas.Queries.GetTipoTareaById;
using Intap.FirstProject.Domain.TiposTareas;
using Intap.FirstProject.Infrastructure.Persistence;
using Intap.FirstProject.Infrastructure.Persistence.Repositories;
using Intap.FirstProject.Tests.Helpers;
using Moq;

namespace Intap.FirstProject.Tests.Application.TiposTareas;

public sealed class GetTipoTareaByIdQueryHandlerTests
{
    private static GetTipoTareaByIdQueryHandler CreateHandler(AppDbContext context)
    {
        Mock<AutoMapper.IMapper> mapper = new();
        mapper.Setup(m => m.Map<TipoTareaDto>(It.IsAny<TipoTarea>()))
              .Returns((TipoTarea t) => new TipoTareaDto(t.Id, t.Nombre, t.Descripcion, t.Estado, t.CreatedAt, t.UpdatedAt));
        return new(new TipoTareaReadRepository(context), mapper.Object);
    }

    // Happy path: existing ID returns the DTO
    [Fact]
    public async Task Handle_WithExistingId_ReturnsTipoTareaDto()
    {
        AppDbContext context = TestDbContextFactory.Create();
        TipoTarea t = TipoTarea.Create("Diseño", "Desc");
        context.TiposTarea.Add(t);
        await context.SaveChangesAsync();

        GetTipoTareaByIdQueryHandler handler = CreateHandler(context);
        Result<TipoTareaDto> result = await handler.Handle(
            new GetTipoTareaByIdQuery(t.Id),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(t.Id);
        result.Value.Nombre.Should().Be("Diseño");
        result.Value.Descripcion.Should().Be("Desc");
        result.Value.Estado.Should().BeTrue();
    }

    // Edge case: non-existing ID returns NotFound
    [Fact]
    public async Task Handle_WithNonExistingId_ReturnsNotFound()
    {
        AppDbContext context = TestDbContextFactory.Create();
        GetTipoTareaByIdQueryHandler handler = CreateHandler(context);

        Result<TipoTareaDto> result = await handler.Handle(
            new GetTipoTareaByIdQuery(Guid.NewGuid()),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorTypeResult.NotFound);
        result.Error.Code.Should().Be("TIPOS_TAREAS_NOT_FOUND");
    }
}
