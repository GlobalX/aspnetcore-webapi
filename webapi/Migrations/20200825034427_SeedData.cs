using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace webapi.Migrations
{
    public partial class SeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "tenants",
                columns: new[] { "Id", "Name", "CreatedAt" },
                values: new object[] { new Guid("a5bab93a-1f7d-4fae-8bf6-ad7f2d6838fe"), "FirstTenant", DateTime.Now});
            
            migrationBuilder.InsertData(
                table: "tenants",
                columns: new[] { "Id", "Name", "CreatedAt" },
                values: new object[] { new Guid("e8124742-65da-485c-a977-0666bc337f5b"), "SecondTenant", DateTime.Now});
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DeleteData(
                table: "Tenants",
                keyColumn: "Id",
                keyValue: "137f5fc6-bb19-4f1e-b3dd-d9b919d932b1");

            migrationBuilder.DeleteData(
                table: "Tenants",
                keyColumn: "Id",
                keyValue: "1ca30a725-19e9-4708-a283-9ad9f4e8c164");
        }
    }
}
