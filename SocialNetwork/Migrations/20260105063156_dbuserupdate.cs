using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialNetwork.Migrations
{
    public partial class dbuserupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "quocgia",
                table: "users",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "quocgia",
                table: "users");
        }
    }
}
