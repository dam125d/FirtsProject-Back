using FluentAssertions;
using Intap.FirstProject.Application.Abstractions.Results;
using Intap.FirstProject.Application.UseCases.SubCategorias.Commands.DeactivateSubCategoria;
using Intap.FirstProject.Domain.Categorias;
using Intap.FirstProject.Domain.SubCategorias;
using Intap.FirstProject.Infrastructure.Persistence;
using Intap.FirstProject.Infrastructure.Persistence.Repositories;
using Intap.FirstProject.Tests.Helpers;
using Microsoft.Extensions.Logging.Abstractions;

namespace Intap.FirstProject.Tests.Application.SubCategorias;

public sealed class DeactivateSubCategoriaCommandHandlerTests
{
    private static DeactivateSubCategoriaCommandHandler CreateHandler(AppDbContext context) =>
        new(new SubCategoriaWriteRepository(context),
            new UnitOfWork(context),
            NullLogger<DeactivateSubCategoriaCommandHandler>.Instance);

    private static async Task<SubCategoria> SeedActiveSubCategoriaAsync(AppDbContext context)
    {
        Categoria categoria = Categoria.Create("Tecnología", null);
        context.Categorias.Add(categoria);

        SubCategoria subCategoria = SubCategoria.Create("Laptops", null, categoria.Id);
        context.SubCategorias.Add(subCategoria);
        await context.SaveChangesAsync();
        return subCategoria;
    }

    // Happy path: active SubCategoria → Estado=false, UpdatedAt set
    [Fact]
    public async Task Handle_WithActiveSubCategoria_Deactivates()
    {
        AppDbContext context = TestDbContextFactory.Create();
        SubCategoria subCategoria = await SeedActiveSubCategoriaAsync(context);
        DeactivateSubCategoriaCommandHandler handler = CreateHandler(context);

        Result result = await handler.Handle(new DeactivateSubCategoriaCommand(subCategoria.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        SubCategoria? updated = context.SubCategorias.FirstOrDefault(s => s.Id == subCategoria.Id);
        updated!.Estado.Should().BeFalse();
        updated.UpdatedAt.Should().NotBeNull();
    }

    // Edge case: already inactive → idempotent 204, no DB write
    [Fact]
    public async Task Handle_WithAlreadyInactiveSubCategoria_ReturnsSuccessIdempotently()
    {
        AppDbContext context = TestDbContextFactory.Create();
        SubCategoria subCategoria = await SeedActiveSubCategoriaAsync(context);

        // First deactivation
        DeactivateSubCategoriaCommandHandler handler = CreateHandler(context);
        await handler.Handle(new DeactivateSubCategoriaCommand(subCategoria.Id), CancellationToken.None);

        DateTime? firstUpdatedAt = context.SubCategorias.First(s => s.Id == subCategoria.Id).UpdatedAt;

        // Second deactivation (idempotent)
        Result result = await handler.Handle(new DeactivateSubCategoriaCommand(subCategoria.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        // UpdatedAt must NOT have changed (no second DB write)
        DateTime? secondUpdatedAt = context.SubCategorias.First(s => s.Id == subCategoria.Id).UpdatedAt;
        secondUpdatedAt.Should().Be(firstUpdatedAt);
    }

    // Edge case: non-existing Id → 404
    [Fact]
    public async Task Handle_WithNonExistingId_ReturnsNotFound()
    {
        AppDbContext context = TestDbContextFactory.Create();
        DeactivateSubCategoriaCommandHandler handler = CreateHandler(context);

        Result result = await handler.Handle(new DeactivateSubCategoriaCommand(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorTypeResult.NotFound);
        result.Error.Code.Should().Be("SUBCATEGORIA_NOT_FOUND");
    }
}
