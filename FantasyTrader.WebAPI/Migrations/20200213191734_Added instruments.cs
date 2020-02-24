using Microsoft.EntityFrameworkCore.Migrations;

namespace FantasyTrader.WebAPI.Migrations
{
    public partial class Addedinstruments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Orders",
                newName: "LimitPrice");

            migrationBuilder.AddColumn<decimal>(
                name: "AverageFillPrice",
                table: "Orders",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "Instruments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Symbol = table.Column<string>(nullable: true),
                    InstrumentPriceSource = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instruments", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Instruments");

            migrationBuilder.DropColumn(
                name: "AverageFillPrice",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "LimitPrice",
                table: "Orders",
                newName: "Price");
        }
    }
}
