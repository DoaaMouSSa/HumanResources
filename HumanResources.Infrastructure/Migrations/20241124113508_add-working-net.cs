using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HumanResources.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addworkingnet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkingHoursTime",
                table: "AttendanceTbl");

            migrationBuilder.AddColumn<long>(
                name: "NetWorkingHours",
                table: "AttendanceTbl",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NetWorkingHours",
                table: "AttendanceTbl");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "WorkingHoursTime",
                table: "AttendanceTbl",
                type: "time",
                nullable: true);
        }
    }
}
