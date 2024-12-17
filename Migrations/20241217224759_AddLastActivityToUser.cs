using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppDev2Project.Migrations
{
    /// <inheritdoc />
    public partial class AddLastActivityToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Options",
                table: "questions");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastActivity",
                table: "users",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastActivity",
                table: "users");

            migrationBuilder.AddColumn<string>(
                name: "Options",
                table: "questions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
