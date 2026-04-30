using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intap.FirstProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTiposTarea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TiposTarea",
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
                    table.PrimaryKey("PK_TiposTarea", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TiposTarea_Estado",
                table: "TiposTarea",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "UQ_TiposTarea_Nombre_Activos",
                table: "TiposTarea",
                column: "Nombre",
                unique: true,
                filter: "\"Estado\" = TRUE");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TiposTarea");
        }
    }
}
