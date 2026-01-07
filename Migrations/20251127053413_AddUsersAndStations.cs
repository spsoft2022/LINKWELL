using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkwellProductionSystem.Migrations
{
    public partial class AddUsersAndStations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AppUsers",
                columns: new[] { "Id", "FullName", "PasswordHash", "Role", "StationId", "Username" },
                values: new object[] { 1, "Administrator", "$2a$11$Wj8i7g5v5j5m9k3n7p2q8e9r0t1y2u3i4o5p6a7s8d9f0g1h2j3k4l", "Admin", null, "admin" });

            migrationBuilder.InsertData(
                table: "Stations",
                columns: new[] { "Id", "IsActive", "Location", "StationCode", "StationName" },
                values: new object[] { 1, true, "Plant A", "ASSY01", "Assembly Line 01" });

            migrationBuilder.InsertData(
                table: "Stations",
                columns: new[] { "Id", "IsActive", "Location", "StationCode", "StationName" },
                values: new object[] { 2, true, "Plant A", "QC01", "Quality Check 01" });

            migrationBuilder.InsertData(
                table: "AppUsers",
                columns: new[] { "Id", "FullName", "PasswordHash", "Role", "StationId", "Username" },
                values: new object[] { 2, "John - ASSY01", "$2a$11$ZmNlMjQwOWRkMjE0YjYxOOU5ZjQ5ZmU5NjY0NjY0NjY0NjY0NjY0Ng==", "Incharge", 1, "assy01" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Stations",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
