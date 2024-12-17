using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HumanResources.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addsalarybefore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "SalaryBeforeAdditon",
                table: "AttendanceTbl",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalaryBeforeAdditon",
                table: "AttendanceTbl");
        }
    }
}
