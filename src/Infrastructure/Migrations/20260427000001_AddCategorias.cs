using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Intap.FirstProject.Infrastructure.Persistence;

#nullable disable

#pragma warning disable CA1814

namespace Intap.FirstProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(AppDbContext))]
    [Migration("20260427000001_AddCategorias")]
    public partial class AddCategorias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ── Tabla Categorias ─────────────────────────────────────────────
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id          = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre      = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Estado      = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt   = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt   = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "UQ_Categorias_Nombre",
                table: "Categorias",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_Estado",
                table: "Categorias",
                column: "Estado");

            // ── FK CategoriaId en Projects ────────────────────────────────────
            migrationBuilder.AddColumn<Guid>(
                name: "CategoriaId",
                table: "Projects",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Categorias_CategoriaId",
                table: "Projects",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CategoriaId",
                table: "Projects",
                column: "CategoriaId");

            // ── Seed permisos RBAC ────────────────────────────────────────────
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Code", "Module", "Name" },
                columnTypes: new[] { "character varying(100)", "character varying(50)", "character varying(150)" },
                values: new object[,]
                {
                    { "view_categorias",   "Categorias", "View Categorias"   },
                    { "create_categorias", "Categorias", "Create Categorias" },
                    { "edit_categorias",   "Categorias", "Edit Categorias"   },
                    { "delete_categorias", "Categorias", "Delete Categorias" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // ── Revertir seed permisos ────────────────────────────────────────
            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Code",
                keyColumnType: "character varying(100)",
                keyValue: "view_categorias");

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Code",
                keyColumnType: "character varying(100)",
                keyValue: "create_categorias");

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Code",
                keyColumnType: "character varying(100)",
                keyValue: "edit_categorias");

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Code",
                keyColumnType: "character varying(100)",
                keyValue: "delete_categorias");

            // ── Revertir FK y columna en Projects ─────────────────────────────
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Categorias_CategoriaId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_CategoriaId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "CategoriaId",
                table: "Projects");

            // ── Revertir tabla Categorias ─────────────────────────────────────
            migrationBuilder.DropTable(name: "Categorias");
        }
    }
}
