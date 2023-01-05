using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TamboliyaApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdGameAddDateEndField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "80a84623-472d-4f80-afce-da4974ac3412");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9132b5dc-b4ee-4921-9cb1-7c59a216dffc");

            migrationBuilder.RenameColumn(
                name: "DateEnd",
                table: "Games",
                newName: "DateEnding");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "8c18186b-bfda-4179-868e-31151471e00c", "2f43d5e2-0a16-446b-af3b-718a63f89267", "Admin", "ADMIN" },
                    { "aabdc93d-3e63-49b4-8ebc-58141d52a0af", "b46136df-5ef1-467e-ae36-091d9870e026", "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8c18186b-bfda-4179-868e-31151471e00c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "aabdc93d-3e63-49b4-8ebc-58141d52a0af");

            migrationBuilder.RenameColumn(
                name: "DateEnding",
                table: "Games",
                newName: "DateEnd");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "80a84623-472d-4f80-afce-da4974ac3412", "ed422a06-e7ad-4ce1-8455-b83530ceaa34", "Admin", "ADMIN" },
                    { "9132b5dc-b4ee-4921-9cb1-7c59a216dffc", "4c88a6d2-bc3c-4356-b8b4-eef88ccd35d1", "User", "USER" }
                });
        }
    }
}
