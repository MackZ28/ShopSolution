using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AuthenticationService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("0e6bee01-d5ca-4fc6-ac39-84004b1aa6d1"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("8e74dad6-91f8-45d0-824a-684b75ce9562"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "CreatedAt", "Description", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("0e6bee01-d5ca-4fc6-ac39-84004b1aa6d1"), null, new DateTime(2025, 10, 16, 11, 54, 48, 212, DateTimeKind.Utc).AddTicks(8166), "Regular user role", "User", "USER" },
                    { new Guid("8e74dad6-91f8-45d0-824a-684b75ce9562"), null, new DateTime(2025, 10, 16, 11, 54, 48, 212, DateTimeKind.Utc).AddTicks(7323), "Administrator role", "Admin", "ADMIN" }
                });
        }
    }
}
