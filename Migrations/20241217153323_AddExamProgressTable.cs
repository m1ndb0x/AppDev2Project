using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppDev2Project.Migrations
{
    /// <inheritdoc />
    public partial class AddExamProgressTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_examProgresses_exams_ExamId",
                table: "examProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_examProgresses_users_UserId",
                table: "examProgresses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_examProgresses",
                table: "examProgresses");

            migrationBuilder.RenameTable(
                name: "examProgresses",
                newName: "exam_progress");

            migrationBuilder.RenameIndex(
                name: "IX_examProgresses_UserId",
                table: "exam_progress",
                newName: "IX_exam_progress_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_examProgresses_ExamId",
                table: "exam_progress",
                newName: "IX_exam_progress_ExamId");

            migrationBuilder.AlterColumn<string>(
                name: "SavedAnswers",
                table: "exam_progress",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsCompleted",
                table: "exam_progress",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddPrimaryKey(
                name: "PK_exam_progress",
                table: "exam_progress",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_exam_progress_exams_ExamId",
                table: "exam_progress",
                column: "ExamId",
                principalTable: "exams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_exam_progress_users_UserId",
                table: "exam_progress",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_exam_progress_exams_ExamId",
                table: "exam_progress");

            migrationBuilder.DropForeignKey(
                name: "FK_exam_progress_users_UserId",
                table: "exam_progress");

            migrationBuilder.DropPrimaryKey(
                name: "PK_exam_progress",
                table: "exam_progress");

            migrationBuilder.RenameTable(
                name: "exam_progress",
                newName: "examProgresses");

            migrationBuilder.RenameIndex(
                name: "IX_exam_progress_UserId",
                table: "examProgresses",
                newName: "IX_examProgresses_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_exam_progress_ExamId",
                table: "examProgresses",
                newName: "IX_examProgresses_ExamId");

            migrationBuilder.AlterColumn<string>(
                name: "SavedAnswers",
                table: "examProgresses",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<bool>(
                name: "IsCompleted",
                table: "examProgresses",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_examProgresses",
                table: "examProgresses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_examProgresses_exams_ExamId",
                table: "examProgresses",
                column: "ExamId",
                principalTable: "exams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_examProgresses_users_UserId",
                table: "examProgresses",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
