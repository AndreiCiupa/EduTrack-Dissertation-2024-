using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduTrack.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedStudentsToTeachers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mark_Student_StudentId",
                table: "Mark");

            migrationBuilder.DropForeignKey(
                name: "FK_Mark_Subject_SubjectId",
                table: "Mark");

            migrationBuilder.DropForeignKey(
                name: "FK_Mark_Teacher_TeacherId",
                table: "Mark");

            migrationBuilder.DropForeignKey(
                name: "FK_Teacher_Student_StudentId",
                table: "Teacher");

            migrationBuilder.DropIndex(
                name: "IX_Teacher_StudentId",
                table: "Teacher");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "Teacher");

            migrationBuilder.AlterColumn<int>(
                name: "TeacherId",
                table: "Mark",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "SubjectId",
                table: "Mark",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "StudentId",
                table: "Mark",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "StudentTeacher",
                columns: table => new
                {
                    StudentsId = table.Column<int>(type: "int", nullable: false),
                    TeachersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentTeacher", x => new { x.StudentsId, x.TeachersId });
                    table.ForeignKey(
                        name: "FK_StudentTeacher_Student_StudentsId",
                        column: x => x.StudentsId,
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentTeacher_Teacher_TeachersId",
                        column: x => x.TeachersId,
                        principalTable: "Teacher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentTeacher_TeachersId",
                table: "StudentTeacher",
                column: "TeachersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Mark_Student_StudentId",
                table: "Mark",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Mark_Subject_SubjectId",
                table: "Mark",
                column: "SubjectId",
                principalTable: "Subject",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Mark_Teacher_TeacherId",
                table: "Mark",
                column: "TeacherId",
                principalTable: "Teacher",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mark_Student_StudentId",
                table: "Mark");

            migrationBuilder.DropForeignKey(
                name: "FK_Mark_Subject_SubjectId",
                table: "Mark");

            migrationBuilder.DropForeignKey(
                name: "FK_Mark_Teacher_TeacherId",
                table: "Mark");

            migrationBuilder.DropTable(
                name: "StudentTeacher");

            migrationBuilder.AddColumn<int>(
                name: "StudentId",
                table: "Teacher",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TeacherId",
                table: "Mark",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SubjectId",
                table: "Mark",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StudentId",
                table: "Mark",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teacher_StudentId",
                table: "Teacher",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Mark_Student_StudentId",
                table: "Mark",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mark_Subject_SubjectId",
                table: "Mark",
                column: "SubjectId",
                principalTable: "Subject",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mark_Teacher_TeacherId",
                table: "Mark",
                column: "TeacherId",
                principalTable: "Teacher",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Teacher_Student_StudentId",
                table: "Teacher",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id");
        }
    }
}
