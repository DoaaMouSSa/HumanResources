using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HumanResources.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateemployeetbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "EmployeeTbl",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateOfAppointment",
                table: "EmployeeTbl",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<int>(
                name: "Governorate",
                table: "EmployeeTbl",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "JobPosition",
                table: "EmployeeTbl",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaritalStatus",
                table: "EmployeeTbl",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "EmployeeTbl",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "EmployeeTbl");

            migrationBuilder.DropColumn(
                name: "DateOfAppointment",
                table: "EmployeeTbl");

            migrationBuilder.DropColumn(
                name: "Governorate",
                table: "EmployeeTbl");

            migrationBuilder.DropColumn(
                name: "JobPosition",
                table: "EmployeeTbl");

            migrationBuilder.DropColumn(
                name: "MaritalStatus",
                table: "EmployeeTbl");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "EmployeeTbl");
        }
    }
}
