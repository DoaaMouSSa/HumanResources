using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HumanResources.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class rmadditiondelay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DelaysHours",
                table: "AttendanceTbl");

            migrationBuilder.DropColumn(
                name: "DelaysTime",
                table: "AttendanceTbl");

            migrationBuilder.DropColumn(
                name: "OverTime",
                table: "AttendanceTbl");

            migrationBuilder.DropColumn(
                name: "OverTimeHours",
                table: "AttendanceTbl");

            migrationBuilder.DropColumn(
                name: "OverTimeSalary",
                table: "AttendanceTbl");

            migrationBuilder.DropColumn(
                name: "deductionSalary",
                table: "AttendanceTbl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DelaysHours",
                table: "AttendanceTbl",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "DelaysTime",
                table: "AttendanceTbl",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "OverTime",
                table: "AttendanceTbl",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OverTimeHours",
                table: "AttendanceTbl",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OverTimeSalary",
                table: "AttendanceTbl",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "deductionSalary",
                table: "AttendanceTbl",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
