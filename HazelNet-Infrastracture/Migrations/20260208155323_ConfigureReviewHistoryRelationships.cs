using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HazelNet_Infrastractire.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureReviewHistoryRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Decks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ReviewHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    CardId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewHistory_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    EmailAddress = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReviewLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Rating = table.Column<short>(type: "smallint", nullable: false),
                    ScheduledDays = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    ElapsedDays = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Review = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    State = table.Column<short>(type: "smallint", nullable: false),
                    ReviewHistoryId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewLog_ReviewHistory_ReviewHistoryId",
                        column: x => x.ReviewHistoryId,
                        principalTable: "ReviewHistory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Decks_UserId",
                table: "Decks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewHistory_CardId",
                table: "ReviewHistory",
                column: "CardId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReviewLog_ReviewHistoryId",
                table: "ReviewLog",
                column: "ReviewHistoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Decks_User_UserId",
                table: "Decks",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Decks_User_UserId",
                table: "Decks");

            migrationBuilder.DropTable(
                name: "ReviewLog");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "ReviewHistory");

            migrationBuilder.DropIndex(
                name: "IX_Decks_UserId",
                table: "Decks");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Decks");
        }
    }
}
