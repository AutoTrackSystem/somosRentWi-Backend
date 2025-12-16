using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SomosRentWi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPricePerHourToRentals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.AddColumn<decimal>(
                name: "PricePerHour",
                table: "Rentals",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.DropColumn(
                name: "PricePerHour",
                table: "Rentals");
        }
    }
}
