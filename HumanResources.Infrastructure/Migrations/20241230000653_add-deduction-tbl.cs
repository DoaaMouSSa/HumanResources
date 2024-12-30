using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HumanResources.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class adddeductiontbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deduction_EmployeeTbl_EmployeeId",
                table: "Deduction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Deduction",
                table: "Deduction");

            migrationBuilder.RenameTable(
                name: "Deduction",
                newName: "DeductionTbl");

            migrationBuilder.RenameIndex(
                name: "IX_Deduction_EmployeeId",
                table: "DeductionTbl",
                newName: "IX_DeductionTbl_EmployeeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeductionTbl",
                table: "DeductionTbl",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DeductionTbl_EmployeeTbl_EmployeeId",
                table: "DeductionTbl",
                column: "EmployeeId",
                principalTable: "EmployeeTbl",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeductionTbl_EmployeeTbl_EmployeeId",
                table: "DeductionTbl");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeductionTbl",
                table: "DeductionTbl");

            migrationBuilder.RenameTable(
                name: "DeductionTbl",
                newName: "Deduction");

            migrationBuilder.RenameIndex(
                name: "IX_DeductionTbl_EmployeeId",
                table: "Deduction",
                newName: "IX_Deduction_EmployeeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Deduction",
                table: "Deduction",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Deduction_EmployeeTbl_EmployeeId",
                table: "Deduction",
                column: "EmployeeId",
                principalTable: "EmployeeTbl",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
