using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace webapi.Migrations
{
    public partial class SeedBooks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "books",
                columns: new[] {"Id", "Title", "TenantId", "Year", "CreatedAt"},
                values: new object[]
                    {Guid.NewGuid(), "MyFirstBook", new Guid("a5bab93a-1f7d-4fae-8bf6-ad7f2d6838fe"), DateTime.Now, DateTime.Now});
            
            migrationBuilder.InsertData(
                table: "books",
                columns: new[] {"Id", "Title", "TenantId", "Year", "CreatedAt"},
                values: new object[]
                    {Guid.NewGuid(), "MySecondBook", new Guid("e8124742-65da-485c-a977-0666bc337f5b"), DateTime.Now, DateTime.Now});
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
