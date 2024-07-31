using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Server.Migrations
{
    /// <inheritdoc />
    public partial class Delete_Sector : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropTable(
                name: "Sectors");

            migrationBuilder.RenameColumn(
                name: "SectorId",
                table: "Routes",
                newName: "DistrictId");

            migrationBuilder.RenameIndex(
                name: "IX_Routes_SectorId",
                table: "Routes",
                newName: "IX_Routes_DistrictId");

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_Districts_DistrictId",
                table: "Routes",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Routes_Districts_DistrictId",
                table: "Routes");

            migrationBuilder.RenameColumn(
                name: "DistrictId",
                table: "Routes",
                newName: "SectorId");

            migrationBuilder.RenameIndex(
                name: "IX_Routes_DistrictId",
                table: "Routes",
                newName: "IX_Routes_SectorId");

            migrationBuilder.CreateTable(
                name: "Sectors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DistrictId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sectors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sectors_Districts_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "Districts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Sectors_DistrictId",
                table: "Sectors",
                column: "DistrictId");

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_Sectors_SectorId",
                table: "Routes",
                column: "SectorId",
                principalTable: "Sectors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
