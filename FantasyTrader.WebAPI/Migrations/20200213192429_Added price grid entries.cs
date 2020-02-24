using Microsoft.EntityFrameworkCore.Migrations;

namespace FantasyTrader.WebAPI.Migrations
{
    public partial class Addedpricegridentries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PriceGridEntries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PriceGridId = table.Column<int>(nullable: true),
                    Symbol = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceGridEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriceGridEntries_PriceGrids_PriceGridId",
                        column: x => x.PriceGridId,
                        principalTable: "PriceGrids",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PriceGridEntries_PriceGridId",
                table: "PriceGridEntries",
                column: "PriceGridId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PriceGridEntries");
        }
    }
}
