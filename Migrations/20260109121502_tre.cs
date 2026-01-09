using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BilkoNavigator_.Migrations
{
    /// <inheritdoc />
    public partial class tre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HerbFindings_AspNetUsers_UserId1",
                table: "HerbFindings");

            migrationBuilder.DropIndex(
                name: "IX_HerbFindings_UserId1",
                table: "HerbFindings");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "HerbFindings");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "HerbFindings",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.CreateIndex(
                name: "IX_HerbFindings_UserId",
                table: "HerbFindings",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_HerbFindings_AspNetUsers_UserId",
                table: "HerbFindings",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HerbFindings_AspNetUsers_UserId",
                table: "HerbFindings");

            migrationBuilder.DropIndex(
                name: "IX_HerbFindings_UserId",
                table: "HerbFindings");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "HerbFindings",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "HerbFindings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HerbFindings_UserId1",
                table: "HerbFindings",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_HerbFindings_AspNetUsers_UserId1",
                table: "HerbFindings",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
