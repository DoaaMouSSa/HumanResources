using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HumanResources.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removeemployeetable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeTbl_DepartmentTbl_DepartmentId",
                table: "EmployeeTbl");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeeTbl",
                table: "EmployeeTbl");

            migrationBuilder.RenameTable(
                name: "EmployeeTbl",
                newName: "Employee");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeTbl_DepartmentId",
                table: "Employee",
                newName: "IX_Employee_DepartmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Employee",
                table: "Employee",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_DepartmentTbl_DepartmentId",
                table: "Employee",
                column: "DepartmentId",
                principalTable: "DepartmentTbl",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_DepartmentTbl_DepartmentId",
                table: "Employee");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Employee",
                table: "Employee");

            migrationBuilder.RenameTable(
                name: "Employee",
                newName: "EmployeeTbl");

            migrationBuilder.RenameIndex(
                name: "IX_Employee_DepartmentId",
                table: "EmployeeTbl",
                newName: "IX_EmployeeTbl_DepartmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployeeTbl",
                table: "EmployeeTbl",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeTbl_DepartmentTbl_DepartmentId",
                table: "EmployeeTbl",
                column: "DepartmentId",
                principalTable: "DepartmentTbl",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
