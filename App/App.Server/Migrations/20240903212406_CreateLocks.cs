using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Server.Migrations
{
    /// <inheritdoc />
    public partial class CreateLocks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Locked",
                table: "Plans");

            migrationBuilder.UpdateData(
                table: "Districts",
                keyColumn: "Name",
                keyValue: null,
                column: "Name",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Districts",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Locks",
                columns: table => new
                {
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    DistrictId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locks", x => new { x.Date, x.DistrictId });
                    table.ForeignKey(
                        name: "FK_Locks_Districts_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "Districts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Locks_DistrictId",
                table: "Locks",
                column: "DistrictId");

            migrationBuilder.Sql(@"
                SET GLOBAL event_scheduler = ON;

                CREATE EVENT IF NOT EXISTS DeleteOldLocks
                ON SCHEDULE EVERY 1 DAY
                STARTS '2024-09-04 00:00:00'
                DO
                BEGIN
                    DELETE FROM Locks
                    WHERE Date < CURDATE();
                END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP EVENT IF EXISTS DeleteOldLocks;
            ");

            migrationBuilder.DropTable(
                name: "Locks");

            migrationBuilder.AddColumn<bool>(
                name: "Locked",
                table: "Plans",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Districts",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
