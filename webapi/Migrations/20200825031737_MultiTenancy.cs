using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace webapi.Migrations
{
    public partial class MultiTenancy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "books",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_books_TenantId",
                table: "books",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_books_tenants_TenantId",
                table: "books",
                column: "TenantId",
                principalTable: "tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.Sql("ALTER TABLE tenants ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("ALTER TABLE books ENABLE ROW LEVEL SECURITY;");
            migrationBuilder.Sql("CREATE POLICY tenant_isolation_policy ON tenants USING (\"Id\" = current_setting('app.current_tenant')::UUID);");
            migrationBuilder.Sql("CREATE POLICY tenant_book_isolation_policy ON books USING (\"TenantId\" = current_setting('app.current_tenant')::UUID);");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_books_tenants_TenantId",
                table: "books");

            migrationBuilder.DropTable(
                name: "tenants");

            migrationBuilder.DropIndex(
                name: "IX_books_TenantId",
                table: "books");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "books");
        }
    }
}
