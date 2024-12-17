using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppDev2Project.Migrations
{
    /// <inheritdoc />
    public partial class FixActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableFrom",
                table: "exams");

            migrationBuilder.DropColumn(
                name: "AvailableUntil",
                table: "exams");

            migrationBuilder.AlterColumn<bool>(
                name: "HasStarted",
                table: "exams",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "Duration",
                table: "exams",
                type: "int",
                nullable: false,
                defaultValue: 60,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "exam_progress",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "exam_progress");

            migrationBuilder.AlterColumn<bool>(
                name: "HasStarted",
                table: "exams",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "Duration",
                table: "exams",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 60);

            migrationBuilder.AddColumn<DateTime>(
                name: "AvailableFrom",
                table: "exams",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "AvailableUntil",
                table: "exams",
                type: "datetime2",
                nullable: true);
        }
    }
}
