using Microsoft.EntityFrameworkCore.Migrations;

namespace webapi.Migrations
{
    public partial class CreateAppUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE USER AppUser WITH PASSWORD 'Welcome1';");
            migrationBuilder.Sql("GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO AppUser;");
            migrationBuilder.Sql("GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO AppUser;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP USER AppUser;");
        }
    }
}
