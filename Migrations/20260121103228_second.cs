using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BilkoNavigator_.Migrations
{
    /// <inheritdoc />
    public partial class second : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Herbs_HerbImages_ImageId",
                table: "Herbs");

            migrationBuilder.DropIndex(
                name: "IX_Herbs_ImageId",
                table: "Herbs");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "Herbs");

            migrationBuilder.AddColumn<int>(
                name: "HerbId",
                table: "HerbImages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_HerbImages_HerbId",
                table: "HerbImages",
                column: "HerbId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_HerbImages_Herbs_HerbId",
                table: "HerbImages",
                column: "HerbId",
                principalTable: "Herbs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HerbImages_Herbs_HerbId",
                table: "HerbImages");

            migrationBuilder.DropIndex(
                name: "IX_HerbImages_HerbId",
                table: "HerbImages");

            migrationBuilder.DropColumn(
                name: "HerbId",
                table: "HerbImages");

            migrationBuilder.AddColumn<int>(
                name: "ImageId",
                table: "Herbs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Herbs_ImageId",
                table: "Herbs",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Herbs_HerbImages_ImageId",
                table: "Herbs",
                column: "ImageId",
                principalTable: "HerbImages",
                principalColumn: "Id");
        }
    }
}
