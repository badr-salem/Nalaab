using Microsoft.EntityFrameworkCore.Migrations;

namespace Nalaab.DataAccess.Migrations
{
    public partial class editproduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_AgeGroups_CoverTypeId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_CoverTypeId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CoverTypeId",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "AgeGroupId",
                table: "Products",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Products_AgeGroupId",
                table: "Products",
                column: "AgeGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_AgeGroups_AgeGroupId",
                table: "Products",
                column: "AgeGroupId",
                principalTable: "AgeGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_AgeGroups_AgeGroupId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_AgeGroupId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AgeGroupId",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "CoverTypeId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CoverTypeId",
                table: "Products",
                column: "CoverTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_AgeGroups_CoverTypeId",
                table: "Products",
                column: "CoverTypeId",
                principalTable: "AgeGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
