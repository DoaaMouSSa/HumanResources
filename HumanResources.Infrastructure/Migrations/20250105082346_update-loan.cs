using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HumanResources.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateloan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "payment",
                table: "LoanTbl",
                newName: "payment_unit");

            migrationBuilder.AddColumn<decimal>(
                name: "left",
                table: "LoanTbl",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "paid",
                table: "LoanTbl",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "payment_amount",
                table: "LoanTbl",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "left",
                table: "LoanTbl");

            migrationBuilder.DropColumn(
                name: "paid",
                table: "LoanTbl");

            migrationBuilder.DropColumn(
                name: "payment_amount",
                table: "LoanTbl");

            migrationBuilder.RenameColumn(
                name: "payment_unit",
                table: "LoanTbl",
                newName: "payment");
        }
    }
}
