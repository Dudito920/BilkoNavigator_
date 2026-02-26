using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BilkoNavigator_.Migrations
{
    /// <inheritdoc />
    public partial class next : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LatinName",
                table: "Herbs",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Herbs_LatinName",
                table: "Herbs",
                column: "LatinName",
                unique: true,
                filter: "[LatinName] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Herbs_LatinName",
                table: "Herbs");

            migrationBuilder.AlterColumn<string>(
                name: "LatinName",
                table: "Herbs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
