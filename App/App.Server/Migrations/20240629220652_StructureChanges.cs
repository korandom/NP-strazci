using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Server.Migrations
{
    /// <inheritdoc />
    public partial class StructureChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DistrictId",
                table: "Vehicles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<bool>(
                name: "Locked",
                table: "Plans",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_DistrictId",
                table: "Vehicles",
                column: "DistrictId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Districts_DistrictId",
                table: "Vehicles",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Districts_DistrictId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_DistrictId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "DistrictId",
                table: "Vehicles");

            migrationBuilder.AlterColumn<int>(
                name: "Locked",
                table: "Plans",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");
        }
    }
}
