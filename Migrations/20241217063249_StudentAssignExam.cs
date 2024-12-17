using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppDev2Project.Migrations
{
    /// <inheritdoc />
    public partial class StudentAssignExam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignedStudentIds",
                table: "exams",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "exam_student_assignments",
                columns: table => new
                {
                    AssignedExamsId = table.Column<int>(type: "int", nullable: false),
                    AssignedStudentsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exam_student_assignments", x => new { x.AssignedExamsId, x.AssignedStudentsId });
                    table.ForeignKey(
                        name: "FK_exam_student_assignments_exams_AssignedExamsId",
                        column: x => x.AssignedExamsId,
                        principalTable: "exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_exam_student_assignments_users_AssignedStudentsId",
                        column: x => x.AssignedStudentsId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_exam_student_assignments_AssignedStudentsId",
                table: "exam_student_assignments",
                column: "AssignedStudentsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "exam_student_assignments");

            migrationBuilder.DropColumn(
                name: "AssignedStudentIds",
                table: "exams");
        }
    }
}
