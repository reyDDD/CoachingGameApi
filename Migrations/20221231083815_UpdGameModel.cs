using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TamboliyaApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdGameModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "02488324-8898-4cad-a015-f88abca3c370");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3f6e423b-37bb-4330-af90-f8cc9e5a7bd4");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Games");

            migrationBuilder.RenameColumn(
                name: "Created",
                table: "Games",
                newName: "DateEnd");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorGuid",
                table: "Games",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateBeginning",
                table: "Games",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "GameType",
                table: "Games",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxUsersCount",
                table: "Games",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ParentGameId",
                table: "Games",
                type: "int",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "80a84623-472d-4f80-afce-da4974ac3412", "ed422a06-e7ad-4ce1-8455-b83530ceaa34", "Admin", "ADMIN" },
                    { "9132b5dc-b4ee-4921-9cb1-7c59a216dffc", "4c88a6d2-bc3c-4356-b8b4-eef88ccd35d1", "User", "USER" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Games_ParentGameId",
                table: "Games",
                column: "ParentGameId");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Games_ParentGameId",
                table: "Games",
                column: "ParentGameId",
                principalTable: "Games",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Games_ParentGameId",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Games_ParentGameId",
                table: "Games");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "80a84623-472d-4f80-afce-da4974ac3412");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9132b5dc-b4ee-4921-9cb1-7c59a216dffc");

            migrationBuilder.DropColumn(
                name: "CreatorGuid",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "DateBeginning",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "GameType",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "MaxUsersCount",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "ParentGameId",
                table: "Games");

            migrationBuilder.RenameColumn(
                name: "DateEnd",
                table: "Games",
                newName: "Created");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Games",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "02488324-8898-4cad-a015-f88abca3c370", "054ef2b8-d5c5-4652-b4cb-573e200ca9e5", "User", "USER" },
                    { "3f6e423b-37bb-4330-af90-f8cc9e5a7bd4", "0a643a06-51c4-4a98-a8f4-ab3f3a529276", "Admin", "ADMIN" }
                });
        }
    }
}
