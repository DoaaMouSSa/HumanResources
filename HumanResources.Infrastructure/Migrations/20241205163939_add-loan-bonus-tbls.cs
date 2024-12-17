using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HumanResources.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addloanbonustbls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BonusTbl",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    finished = table.Column<bool>(type: "bit", nullable: false),
                    EmployeeCode = table.Column<int>(type: "int", nullable: true),
                    EmployeeId = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedAt = table.Column<DateOnly>(type: "date", nullable: false),
                    DeletedAt = table.Column<DateOnly>(type: "date", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BonusTbl", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BonusTbl_EmployeeTbl_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "EmployeeTbl",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LoanTbl",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    numberofpayment = table.Column<int>(type: "int", nullable: false),
                    finished = table.Column<bool>(type: "bit", nullable: false),
                    EmployeeCode = table.Column<int>(type: "int", nullable: true),
                    EmployeeId = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedAt = table.Column<DateOnly>(type: "date", nullable: false),
                    DeletedAt = table.Column<DateOnly>(type: "date", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanTbl", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanTbl_EmployeeTbl_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "EmployeeTbl",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BonusTbl_EmployeeId",
                table: "BonusTbl",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanTbl_EmployeeId",
                table: "LoanTbl",
                column: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BonusTbl");

            migrationBuilder.DropTable(
                name: "LoanTbl");
        }
    }
}
