using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SomosRentWi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminUserSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "IsActive", "PasswordHash", "Role" },
                values: new object[] { 4, "admin@somosrentwi.com", true, "PrP+ZrMeO00Q+nC1ytSccRIpSvauTkdqHEBRVdRaoSE=", 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
