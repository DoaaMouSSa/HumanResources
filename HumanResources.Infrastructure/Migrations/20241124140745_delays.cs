using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HumanResources.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class delays : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NetWorkingHours",
                table: "AttendanceTbl");

            migrationBuilder.DropColumn(
                name: "TotalWorkingMinutes",
                table: "AttendanceTbl");

            migrationBuilder.AlterColumn<double>(
                name: "TotalWorkingHoursBeforeDelays",
                table: "AttendanceTbl",
                type: "float",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "TotalWorkingHours",
                table: "AttendanceTbl",
                type: "float",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "OverTimeHours",
                table: "AttendanceTbl",
                type: "float",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "DelaysHours",
                table: "AttendanceTbl",
                type: "float",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CalculatedSalary",
                table: "AttendanceTbl",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CalculatedSalary",
                table: "AttendanceTbl");

            migrationBuilder.AlterColumn<long>(
                name: "TotalWorkingHoursBeforeDelays",
                table: "AttendanceTbl",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "TotalWorkingHours",
                table: "AttendanceTbl",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "OverTimeHours",
                table: "AttendanceTbl",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "DelaysHours",
                table: "AttendanceTbl",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "NetWorkingHours",
                table: "AttendanceTbl",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TotalWorkingMinutes",
                table: "AttendanceTbl",
                type: "bigint",
                nullable: true);
        }
    }
}
