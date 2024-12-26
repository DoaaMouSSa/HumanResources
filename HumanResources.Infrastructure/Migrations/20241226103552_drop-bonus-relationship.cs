using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HumanResources.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class dropbonusrelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BonusTbl_EmployeeTbl_EmployeeId",
                table: "BonusTbl");

            migrationBuilder.DropIndex(
                name: "IX_BonusTbl_EmployeeId",
                table: "BonusTbl");

            migrationBuilder.DropColumn(
                name: "EmployeeCode",
                table: "BonusTbl");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "BonusTbl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmployeeCode",
                table: "BonusTbl",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "BonusTbl",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BonusTbl_EmployeeId",
                table: "BonusTbl",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_BonusTbl_EmployeeTbl_EmployeeId",
                table: "BonusTbl",
                column: "EmployeeId",
                principalTable: "EmployeeTbl",
                principalColumn: "Id");
        }
    }
}
