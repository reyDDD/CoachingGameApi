using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TamboliyaApi.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "9861a0cb-1798-46fb-8523-4dfeaadd343d", "3e6811bf-56a7-49f9-920b-991959bc406d", "User", "USER" },
                    { "b98f3f04-9709-459e-bb19-b223bf6d8fcf", "1b9aad25-c5c9-4afd-853b-e36e7c4325f7", "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9861a0cb-1798-46fb-8523-4dfeaadd343d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b98f3f04-9709-459e-bb19-b223bf6d8fcf");
        }
    }
}
