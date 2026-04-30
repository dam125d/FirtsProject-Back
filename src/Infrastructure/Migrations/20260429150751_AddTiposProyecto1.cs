using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intap.FirstProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTiposProyecto1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TiposProyecto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Estado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposProyecto", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TiposProyecto_Estado",
                table: "TiposProyecto",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "UQ_TiposProyecto_Nombre_Activos",
                table: "TiposProyecto",
                column: "Nombre",
                unique: true,
                filter: "\"Estado\" = TRUE");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TiposProyecto");
        }
    }
}
