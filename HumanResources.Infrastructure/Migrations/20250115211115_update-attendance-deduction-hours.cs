using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HumanResources.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateattendancedeductionhours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isDeductionBoth",
                table: "AttendanceTbl");

            migrationBuilder.AddColumn<decimal>(
                name: "amountHours",
                table: "DeductionTbl",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DeductionHoursAmount",
                table: "AttendanceTbl",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "amountHours",
                table: "DeductionTbl");

            migrationBuilder.DropColumn(
                name: "DeductionHoursAmount",
                table: "AttendanceTbl");

            migrationBuilder.AddColumn<bool>(
                name: "isDeductionBoth",
                table: "AttendanceTbl",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
