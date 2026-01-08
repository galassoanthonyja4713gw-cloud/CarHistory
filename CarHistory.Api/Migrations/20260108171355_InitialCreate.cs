using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarHistory.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "car_entries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Make = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Model = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Trim = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Color = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Hp = table.Column<int>(type: "integer", nullable: true),
                    Tq = table.Column<int>(type: "integer", nullable: true),
                    Vin = table.Column<string>(type: "character varying(17)", maxLength: 17, nullable: true),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    PurchaseDate = table.Column<DateOnly>(type: "date", nullable: false),
                    SoldDate = table.Column<DateOnly>(type: "date", nullable: true),
                    OdometerAtPurchase = table.Column<int>(type: "integer", nullable: false),
                    OdometerAtSale = table.Column<int>(type: "integer", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_car_entries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_car_entries_Make_Model_Year",
                table: "car_entries",
                columns: new[] { "Make", "Model", "Year" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "car_entries");
        }
    }
}
