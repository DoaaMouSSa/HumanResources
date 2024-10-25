using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HumanResources.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateemptbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NetSalary",
                table: "EmployeeTbl");

            migrationBuilder.AddColumn<int>(
                name: "SalaryFormula",
                table: "EmployeeTbl",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalaryFormula",
                table: "EmployeeTbl");

            migrationBuilder.AddColumn<float>(
                name: "NetSalary",
                table: "EmployeeTbl",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }
    }
}
