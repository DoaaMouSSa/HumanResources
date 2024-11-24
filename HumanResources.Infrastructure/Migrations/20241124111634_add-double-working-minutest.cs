using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HumanResources.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class adddoubleworkingminutest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkingHours",
                table: "AttendanceTbl");

            migrationBuilder.RenameColumn(
                name: "WorkingMinutes",
                table: "AttendanceTbl",
                newName: "TotalWorkingMinutes");

            migrationBuilder.AlterColumn<long>(
                name: "TotalWorkingHours",
                table: "AttendanceTbl",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalWorkingMinutes",
                table: "AttendanceTbl",
                newName: "WorkingMinutes");

            migrationBuilder.AlterColumn<double>(
                name: "TotalWorkingHours",
                table: "AttendanceTbl",
                type: "float",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "WorkingHours",
                table: "AttendanceTbl",
                type: "bigint",
                nullable: true);
        }
    }
}
