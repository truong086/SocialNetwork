using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialNetwork.Migrations
{
    public partial class columnewpostimage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "image_user_id",
                table: "Post_Images",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Post_Images_image_user_id",
                table: "Post_Images",
                column: "image_user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Images_image_Users_image_user_id",
                table: "Post_Images",
                column: "image_user_id",
                principalTable: "image_Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_Images_image_Users_image_user_id",
                table: "Post_Images");

            migrationBuilder.DropIndex(
                name: "IX_Post_Images_image_user_id",
                table: "Post_Images");

            migrationBuilder.DropColumn(
                name: "image_user_id",
                table: "Post_Images");
        }
    }
}
