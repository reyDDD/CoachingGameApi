using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TamboliyaApi.Migrations
{
    public partial class BaseModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsFinished = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ActualPositionsOnMapForSelect",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegionOnMap = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PositionNumber = table.Column<int>(type: "int", nullable: false),
                    IsSelected = table.Column<bool>(type: "bit", nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "ActualPositionsOnTheMap",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegionOnMap = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PositionNumber = table.Column<int>(type: "int", nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActualPositionsOnTheMap", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActualPositionsOnTheMap_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InitialGamesData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Motive = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QualityOfExperience = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EnvironmentAndCircumstances = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChainLinks = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExitPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegionOnMap = table.Column<int>(type: "int", nullable: false),
                    StepOnPath = table.Column<int>(type: "int", nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InitialGamesData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InitialGamesData_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActualPositionsOnMapForSelect_GameId",
                table: "ActualPositionsOnMapForSelect",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_ActualPositionsOnTheMap_GameId",
                table: "ActualPositionsOnTheMap",
                column: "GameId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InitialGamesData_GameId",
                table: "InitialGamesData",
                column: "GameId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActualPositionsOnMapForSelect");

            migrationBuilder.DropTable(
                name: "ActualPositionsOnTheMap");

            migrationBuilder.DropTable(
                name: "InitialGamesData");

            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}
