using Microsoft.EntityFrameworkCore.Migrations;

namespace PosCFG.Migrations
{
    public partial class newterminal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HeaderLine1",
                table: "Terminals",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeaderLine2",
                table: "Terminals",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeaderLine3",
                table: "Terminals",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeaderLine1",
                table: "Terminals");

            migrationBuilder.DropColumn(
                name: "HeaderLine2",
                table: "Terminals");

            migrationBuilder.DropColumn(
                name: "HeaderLine3",
                table: "Terminals");
        }
    }
}
