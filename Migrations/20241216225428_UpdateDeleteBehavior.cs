using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppDev2Project.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_completed_exams_exams_ExamId",
                table: "completed_exams");

            migrationBuilder.DropForeignKey(
                name: "FK_exam_attempt_questions_QuestionId",
                table: "exam_attempt");

            migrationBuilder.DropForeignKey(
                name: "FK_questions_exams_ExamId",
                table: "questions");

            migrationBuilder.AddForeignKey(
                name: "FK_completed_exams_exams_ExamId",
                table: "completed_exams",
                column: "ExamId",
                principalTable: "exams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_exam_attempt_questions_QuestionId",
                table: "exam_attempt",
                column: "QuestionId",
                principalTable: "questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_questions_exams_ExamId",
                table: "questions",
                column: "ExamId",
                principalTable: "exams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_completed_exams_exams_ExamId",
                table: "completed_exams");

            migrationBuilder.DropForeignKey(
                name: "FK_exam_attempt_questions_QuestionId",
                table: "exam_attempt");

            migrationBuilder.DropForeignKey(
                name: "FK_questions_exams_ExamId",
                table: "questions");

            migrationBuilder.AddForeignKey(
                name: "FK_completed_exams_exams_ExamId",
                table: "completed_exams",
                column: "ExamId",
                principalTable: "exams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_exam_attempt_questions_QuestionId",
                table: "exam_attempt",
                column: "QuestionId",
                principalTable: "questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_questions_exams_ExamId",
                table: "questions",
                column: "ExamId",
                principalTable: "exams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
