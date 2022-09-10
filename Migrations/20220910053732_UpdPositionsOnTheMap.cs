using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TamboliyaApi.Migrations
{
    public partial class UpdPositionsOnTheMap : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActualPositionsOnMapForSelect");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Games",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "GameLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameLogs_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameLogs_GameId",
                table: "GameLogs",
                column: "GameId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameLogs");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Games");

            migrationBuilder.CreateTable(
                name: "ActualPositionsOnMapForSelect",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsSelected = table.Column<bool>(type: "bit", nullable: false),
                    PositionNumber = table.Column<int>(type: "int", nullable: false),
                    RegionOnMap = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActualPositionsOnMapForSelect", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActualPositionsOnMapForSelect_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActualPositionsOnMapForSelect_GameId",
                table: "ActualPositionsOnMapForSelect",
                column: "GameId");
        }
    }
}
