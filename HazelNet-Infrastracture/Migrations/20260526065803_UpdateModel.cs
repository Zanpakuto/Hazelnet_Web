using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HazelNet_Infrastractire.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FSRSParametersId",
                table: "User",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FrontOfCard",
                table: "Cards",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "ReviewHistoryId",
                table: "Cards",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "FSRSParameters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RequestRetention = table.Column<double>(type: "double precision", nullable: false),
                    MaximumInterval = table.Column<double>(type: "double precision", nullable: false),
                    W = table.Column<double[]>(type: "double precision[]", nullable: false),
                    Decay = table.Column<double>(type: "double precision", nullable: false),
                    Factor = table.Column<double>(type: "double precision", nullable: false),
                    EnableShortTerm = table.Column<bool>(type: "boolean", nullable: false),
                    EnableFuzz = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FSRSParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FSRSParameters_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FSRSParameters_UserId",
                table: "FSRSParameters",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FSRSParameters");

            migrationBuilder.DropColumn(
                name: "FSRSParametersId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ReviewHistoryId",
                table: "Cards");

            migrationBuilder.AlterColumn<string>(
                name: "FrontOfCard",
                table: "Cards",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
