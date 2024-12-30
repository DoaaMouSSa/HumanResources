using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HumanResources.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class loan_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanTbl_EmployeeTbl_EmployeeId",
                table: "LoanTbl");

            migrationBuilder.DropIndex(
                name: "IX_LoanTbl_EmployeeId",
                table: "LoanTbl");

            migrationBuilder.DropColumn(
                name: "EmployeeCode",
                table: "LoanTbl");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "LoanTbl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmployeeCode",
                table: "LoanTbl",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "LoanTbl",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoanTbl_EmployeeId",
                table: "LoanTbl",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_LoanTbl_EmployeeTbl_EmployeeId",
                table: "LoanTbl",
                column: "EmployeeId",
                principalTable: "EmployeeTbl",
                principalColumn: "Id");
        }
    }
}
