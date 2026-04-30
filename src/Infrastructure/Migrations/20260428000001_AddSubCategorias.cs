using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814

namespace Intap.FirstProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSubCategorias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ── BD-01: Prerequisite check — Categorias table must exist (created in AddCategorias).
            //    If this migration runs in isolation without AddCategorias the FK below will fail.
            //    The migration chain enforces the correct order.

            // ── BD-02: Tabla SubCategorias ────────────────────────────────────
            migrationBuilder.CreateTable(
                name: "SubCategorias",
                columns: table => new
                {
                    Id          = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre      = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CategoriaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Estado      = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt   = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt   = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubCategorias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubCategorias_Categorias",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // ── BD-03: Filtered unique index — Nombre + CategoriaId among active records ──
            // PostgreSQL supports WHERE clauses on partial indexes natively.
            migrationBuilder.Sql(
                @"CREATE UNIQUE INDEX ""UQ_SubCategorias_Nombre_CategoriaId_Activas""
                  ON ""SubCategorias"" (""Nombre"", ""CategoriaId"")
                  WHERE ""Estado"" = true;");

            // ── BD-04: Support indexes ────────────────────────────────────────
            migrationBuilder.CreateIndex(
                name: "IX_SubCategorias_CategoriaId",
                table: "SubCategorias",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategorias_Estado",
                table: "SubCategorias",
                column: "Estado");

            // ── BD-05: Seed RBAC permissions — idempotent ─────────────────────
            migrationBuilder.Sql(
                @"INSERT INTO ""Permissions"" (""Code"", ""Name"", ""Module"")
                  SELECT v.code, v.name, v.module
                  FROM (VALUES
                    ('subcategorias:read',   'Ver sub-categorías',        'SubCategorias'),
                    ('subcategorias:create', 'Crear sub-categorías',       'SubCategorias'),
                    ('subcategorias:update', 'Modificar sub-categorías',   'SubCategorias'),
                    ('subcategorias:delete', 'Desactivar sub-categorías',  'SubCategorias')
                  ) AS v(code, name, module)
                  WHERE NOT EXISTS (
                    SELECT 1 FROM ""Permissions"" WHERE ""Code"" = v.code
                  );");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // ── DOWN: Revert seed permisos ────────────────────────────────────
            migrationBuilder.Sql(
                @"DELETE FROM ""Permissions""
                  WHERE ""Code"" IN (
                    'subcategorias:read',
                    'subcategorias:create',
                    'subcategorias:update',
                    'subcategorias:delete'
                  );");

            // ── DOWN: Revertir índices de soporte y tabla ─────────────────────
            // UQ index is dropped automatically with DROP TABLE.
            // IX indexes are dropped automatically with DROP TABLE.
            migrationBuilder.DropTable(name: "SubCategorias");
        }
    }
}
