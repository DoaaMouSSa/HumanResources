using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HumanResources.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateloandeduction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "finished",
                table: "LoanTbl",
                newName: "Done");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "LoanTbl",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "payment",
                table: "LoanTbl",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "Deduction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DeductionType = table.Column<int>(type: "int", nullable: false),
                    Done = table.Column<bool>(type: "bit", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedAt = table.Column<DateOnly>(type: "date", nullable: false),
                    DeletedAt = table.Column<DateOnly>(type: "date", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deduction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Deduction_EmployeeTbl_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "EmployeeTbl",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoanTbl_EmployeeId",
                table: "LoanTbl",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Deduction_EmployeeId",
                table: "Deduction",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_LoanTbl_EmployeeTbl_EmployeeId",
                table: "LoanTbl",
                column: "EmployeeId",
                principalTable: "EmployeeTbl",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanTbl_EmployeeTbl_EmployeeId",
                table: "LoanTbl");

            migrationBuilder.DropTable(
                name: "Deduction");

            migrationBuilder.DropIndex(
                name: "IX_LoanTbl_EmployeeId",
                table: "LoanTbl");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "LoanTbl");

            migrationBuilder.DropColumn(
                name: "payment",
                table: "LoanTbl");

            migrationBuilder.RenameColumn(
                name: "Done",
                table: "LoanTbl",
                newName: "finished");
        }
    }
}
