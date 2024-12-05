using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HumanResources.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addcreatedattedance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceTbl_WeekTbl_WeekId",
                table: "AttendanceTbl");

            migrationBuilder.DropTable(
                name: "WeekTbl");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceTbl_WeekId",
                table: "AttendanceTbl");

            migrationBuilder.DropColumn(
                name: "WeekId",
                table: "AttendanceTbl");

            migrationBuilder.AddColumn<string>(
                name: "Created",
                table: "AttendanceTbl",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "AttendanceTbl",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created",
                table: "AttendanceTbl");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "AttendanceTbl");

            migrationBuilder.AddColumn<int>(
                name: "WeekId",
                table: "AttendanceTbl",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WeekTbl",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeekTbl", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceTbl_WeekId",
                table: "AttendanceTbl",
                column: "WeekId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceTbl_WeekTbl_WeekId",
                table: "AttendanceTbl",
                column: "WeekId",
                principalTable: "WeekTbl",
                principalColumn: "Id");
        }
    }
}
