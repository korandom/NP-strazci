using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Server.Migrations
{
    /// <inheritdoc />
    public partial class Attendence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Attendences",
                columns: table => new
                {
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    RangerId = table.Column<int>(type: "int", nullable: false),
                    Working = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    From = table.Column<TimeOnly>(type: "time(6)", nullable: true),
                    ReasonOfAbsence = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReasonOfAbsenceEnum = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendences", x => new { x.Date, x.RangerId });
                    table.ForeignKey(
                        name: "FK_Attendences_Rangers_RangerId",
                        column: x => x.RangerId,
                        principalTable: "Rangers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Attendences_RangerId",
                table: "Attendences",
                column: "RangerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attendences");
        }
    }
}
