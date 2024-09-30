using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HumanResources.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateemployetabe_test2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeTbl_DepartmentTbl_DepartmentId",
                table: "EmployeeTbl");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "EmployeeTbl");

            migrationBuilder.DropColumn(
                name: "BirthOfDate",
                table: "EmployeeTbl");

            migrationBuilder.DropColumn(
                name: "CheckInTime",
                table: "EmployeeTbl");

            migrationBuilder.DropColumn(
                name: "CheckOutTime",
                table: "EmployeeTbl");

            migrationBuilder.DropColumn(
                name: "DateOfAppointment",
                table: "EmployeeTbl");

            migrationBuilder.DropColumn(
                name: "ExperienceLevel",
                table: "EmployeeTbl");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "EmployeeTbl");

            migrationBuilder.DropColumn(
                name: "Governorate",
                table: "EmployeeTbl");

            migrationBuilder.DropColumn(
                name: "GraduationCertificateUrl",
                table: "EmployeeTbl");

            migrationBuilder.DropColumn(
                name: "GrossSalary",
                table: "EmployeeTbl");

            migrationBuilder.DropColumn(
                name: "IdentityUrl",
                table: "EmployeeTbl");

            migrationBuilder.DropColumn(
                name: "JobPosition",
                table: "EmployeeTbl");

            migrationBuilder.DropColumn(
                name: "MaritalStatus",
                table: "EmployeeTbl");

            migrationBuilder.DropColumn(
                name: "NetSalary",
                table: "EmployeeTbl");

            migrationBuilder.DropColumn(
                name: "PersonalImageUrl",
                table: "EmployeeTbl");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "EmployeeTbl");

            migrationBuilder.AlterColumn<int>(
                name: "DepartmentId",
                table: "EmployeeTbl",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeTbl_DepartmentTbl_DepartmentId",
                table: "EmployeeTbl",
                column: "DepartmentId",
                principalTable: "DepartmentTbl",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeTbl_DepartmentTbl_DepartmentId",
                table: "EmployeeTbl");

            migrationBuilder.AlterColumn<int>(
                name: "DepartmentId",
                table: "EmployeeTbl",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "EmployeeTbl",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "BirthOfDate",
                table: "EmployeeTbl",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "CheckInTime",
                table: "EmployeeTbl",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "CheckOutTime",
                table: "EmployeeTbl",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateOfAppointment",
                table: "EmployeeTbl",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExperienceLevel",
                table: "EmployeeTbl",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "EmployeeTbl",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Governorate",
                table: "EmployeeTbl",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GraduationCertificateUrl",
                table: "EmployeeTbl",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "GrossSalary",
                table: "EmployeeTbl",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentityUrl",
                table: "EmployeeTbl",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "JobPosition",
                table: "EmployeeTbl",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaritalStatus",
                table: "EmployeeTbl",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "NetSalary",
                table: "EmployeeTbl",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PersonalImageUrl",
                table: "EmployeeTbl",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "EmployeeTbl",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeTbl_DepartmentTbl_DepartmentId",
                table: "EmployeeTbl",
                column: "DepartmentId",
                principalTable: "DepartmentTbl",
                principalColumn: "Id");
        }
    }
}
