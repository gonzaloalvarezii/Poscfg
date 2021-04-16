using Microsoft.EntityFrameworkCore.Migrations;

namespace PosCFG.Migrations
{
    public partial class onetoone : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SystemPOSs_TerminalID",
                table: "SystemPOSs");

            migrationBuilder.CreateIndex(
                name: "IX_SystemPOSs_TerminalID",
                table: "SystemPOSs",
                column: "TerminalID",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SystemPOSs_TerminalID",
                table: "SystemPOSs");

            migrationBuilder.CreateIndex(
                name: "IX_SystemPOSs_TerminalID",
                table: "SystemPOSs",
                column: "TerminalID");
        }
    }
}
