using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Intap.FirstProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateEmpleados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Empleados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Identificacion = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Nombres = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Apellidos = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Correo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Cargo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Estado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empleados", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Empleados_Cargo",
                table: "Empleados",
                column: "Cargo");

            migrationBuilder.CreateIndex(
                name: "IX_Empleados_Estado",
                table: "Empleados",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "UQ_Empleados_Correo",
                table: "Empleados",
                column: "Correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Empleados_Identificacion",
                table: "Empleados",
                column: "Identificacion",
                unique: true);

            // ── Seed permissions RBAC ────────────────────────────────────
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Code", "Module", "Name" },
                columnTypes: new[] { "character varying(100)", "character varying(50)", "character varying(150)" },
                values: new object[,]
                {
                    { "empleados:read",   "Empleados", "View Empleados"   },
                    { "empleados:create", "Empleados", "Create Empleados" },
                    { "empleados:update", "Empleados", "Update Empleados" },
                    { "empleados:delete", "Empleados", "Delete Empleados" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // ── Revertir seed permisos ────────────────────────────────
            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Code",
                keyColumnType: "character varying(100)",
                keyValue: "empleados:read");

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Code",
                keyColumnType: "character varying(100)",
                keyValue: "empleados:create");

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Code",
                keyColumnType: "character varying(100)",
                keyValue: "empleados:update");

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Code",
                keyColumnType: "character varying(100)",
                keyValue: "empleados:delete");

            migrationBuilder.DropTable(
                name: "Empleados");
        }
    }
}
