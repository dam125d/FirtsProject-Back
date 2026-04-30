using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Intap.FirstProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRbacDynamicRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Create Permissions table
            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Module = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Code);
                });

            // 2. Create Roles table
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false, defaultValue: ""),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsSystem = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            // 3. Create RolePermissions junction table
            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionCode = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionCode });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionCode",
                        column: x => x.PermissionCode,
                        principalTable: "Permissions",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // 4. Seed permissions
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Code", "Module", "Name" },
                values: new object[,]
                {
                    { "create_projects", "Projects", "Create Projects" },
                    { "create_tasks", "Tasks", "Create Tasks" },
                    { "create_users", "Users", "Create Users" },
                    { "delete_projects", "Projects", "Delete Projects" },
                    { "delete_tasks", "Tasks", "Delete Tasks" },
                    { "delete_users", "Users", "Delete Users" },
                    { "edit_projects", "Projects", "Edit Projects" },
                    { "edit_tasks", "Tasks", "Edit Tasks" },
                    { "edit_users", "Users", "Edit Users" },
                    { "view_projects", "Projects", "View Projects" },
                    { "view_tasks", "Tasks", "View Tasks" },
                    { "view_users", "Users", "View Users" }
                });

            // 5. Seed roles
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "IsSystem", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Full system access", true, true, "Admin", null },
                    { new Guid("00000000-0000-0000-0000-000000000002"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Standard user access", true, true, "User", null }
                });

            // 6. Seed Admin role permissions
            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionCode", "RoleId" },
                values: new object[,]
                {
                    { "create_projects", new Guid("00000000-0000-0000-0000-000000000001") },
                    { "create_tasks", new Guid("00000000-0000-0000-0000-000000000001") },
                    { "create_users", new Guid("00000000-0000-0000-0000-000000000001") },
                    { "delete_projects", new Guid("00000000-0000-0000-0000-000000000001") },
                    { "delete_tasks", new Guid("00000000-0000-0000-0000-000000000001") },
                    { "delete_users", new Guid("00000000-0000-0000-0000-000000000001") },
                    { "edit_projects", new Guid("00000000-0000-0000-0000-000000000001") },
                    { "edit_tasks", new Guid("00000000-0000-0000-0000-000000000001") },
                    { "edit_users", new Guid("00000000-0000-0000-0000-000000000001") },
                    { "view_projects", new Guid("00000000-0000-0000-0000-000000000001") },
                    { "view_tasks", new Guid("00000000-0000-0000-0000-000000000001") },
                    { "view_users", new Guid("00000000-0000-0000-0000-000000000001") }
                });

            // 7. Add RoleId as nullable first (to allow backfill)
            migrationBuilder.AddColumn<Guid>(
                name: "RoleId",
                table: "Users",
                type: "uuid",
                nullable: true);

            // 8. Backfill RoleId from old Role string column
            migrationBuilder.Sql(@"
                UPDATE ""Users""
                SET ""RoleId"" = r.""Id""
                FROM ""Roles"" r
                WHERE r.""Name"" = ""Users"".""Role""
                  AND ""Users"".""RoleId"" IS NULL;

                -- Fallback: assign Admin role to any user whose Role string didn't match
                UPDATE ""Users""
                SET ""RoleId"" = '00000000-0000-0000-0000-000000000001'
                WHERE ""RoleId"" IS NULL;
            ");

            // 9. Make RoleId NOT NULL
            migrationBuilder.AlterColumn<Guid>(
                name: "RoleId",
                table: "Users",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            // 10. Drop old Role string column
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            // 11. Create indexes
            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionCode",
                table: "RolePermissions",
                column: "PermissionCode");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            // 12. Add FK from Users to Roles
            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Users_RoleId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
