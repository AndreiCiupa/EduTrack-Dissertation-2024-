using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduTrack.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedPasswordMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Teacher",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Student",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Teacher");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Student");
        }
    }
}
