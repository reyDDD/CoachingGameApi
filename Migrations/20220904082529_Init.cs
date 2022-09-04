using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TamboliyaApi.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SideOfDodecahedrons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<int>(type: "int", nullable: false),
                    Color = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SideOfDodecahedrons", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "SideOfDodecahedrons",
                columns: new[] { "Id", "Color", "Number" },
                values: new object[,]
                {
                    { 1, 2, 1 },
                    { 2, 1, 2 },
                    { 3, 3, 3 },
                    { 4, 3, 4 },
                    { 5, 4, 5 },
                    { 6, 1, 6 },
                    { 7, 4, 7 },
                    { 8, 2, 8 },
                    { 9, 2, 9 },
                    { 10, 4, 10 },
                    { 11, 3, 11 },
                    { 12, 1, 12 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SideOfDodecahedrons");
        }
    }
}
