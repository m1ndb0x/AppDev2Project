using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppDev2Project.Migrations
{
    /// <inheritdoc />
    public partial class OptimizeExamRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_exam_progress_ExamId",
                table: "exam_progress");

            migrationBuilder.AlterColumn<string>(
                name: "SavedAnswers",
                table: "exam_progress",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.CreateIndex(
                name: "IX_exam_progress_ExamId_UserId_IsCompleted_IsActive",
                table: "exam_progress",
                columns: new[] { "ExamId", "UserId", "IsCompleted", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_exam_progress_ExamId_UserId_IsCompleted_IsActive",
                table: "exam_progress");

            migrationBuilder.AlterColumn<string>(
                name: "SavedAnswers",
                table: "exam_progress",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_exam_progress_ExamId",
                table: "exam_progress",
                column: "ExamId");
        }
    }
}
