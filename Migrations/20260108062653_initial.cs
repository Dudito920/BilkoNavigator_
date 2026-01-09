using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BilkoNavigator_.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Herbs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PopularName = table.Column<string>(type: "TEXT", nullable: false),
                    LatinName = table.Column<string>(type: "TEXT", nullable: false),
                    DialectNames = table.Column<string>(type: "TEXT", nullable: false),
                    Aroma = table.Column<string>(type: "TEXT", nullable: false),
                    Taste = table.Column<string>(type: "TEXT", nullable: false),
                    Habitat = table.Column<string>(type: "TEXT", nullable: false),
                    Season = table.Column<string>(type: "TEXT", nullable: false),
                    IsPoisonous = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsProtected = table.Column<bool>(type: "INTEGER", nullable: false),
                    UsedPart = table.Column<string>(type: "TEXT", nullable: false),
                    Benefits = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Herbs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Latitude = table.Column<double>(type: "REAL", nullable: false),
                    Longitude = table.Column<double>(type: "REAL", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    UserStatus = table.Column<string>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HerbFindings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    HerbId = table.Column<int>(type: "INTEGER", nullable: false),
                    LocationId = table.Column<int>(type: "INTEGER", nullable: false),
                    FoundOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserId1 = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HerbFindings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HerbFindings_Herbs_HerbId",
                        column: x => x.HerbId,
                        principalTable: "Herbs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HerbFindings_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HerbFindings_Users_UserId1",
                        column: x => x.UserId1,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HerbImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: false),
                    UploadedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    HerbId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId1 = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HerbImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HerbImages_Herbs_HerbId",
                        column: x => x.HerbId,
                        principalTable: "Herbs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HerbImages_Users_UserId1",
                        column: x => x.UserId1,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_HerbFindings_HerbId",
                table: "HerbFindings",
                column: "HerbId");

            migrationBuilder.CreateIndex(
                name: "IX_HerbFindings_LocationId",
                table: "HerbFindings",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_HerbFindings_UserId1",
                table: "HerbFindings",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_HerbImages_HerbId",
                table: "HerbImages",
                column: "HerbId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HerbImages_UserId1",
                table: "HerbImages",
                column: "UserId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HerbFindings");

            migrationBuilder.DropTable(
                name: "HerbImages");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "Herbs");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
