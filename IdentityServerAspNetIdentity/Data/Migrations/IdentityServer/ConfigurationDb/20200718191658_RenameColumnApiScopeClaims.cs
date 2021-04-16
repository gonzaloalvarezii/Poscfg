using Microsoft.EntityFrameworkCore.Migrations;

namespace IdentityServerAspNetIdentity.Data.Migrations.IdentityServer.ConfigurationDb
{
    public partial class RenameColumnApiScopeClaims : Migration
    {
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApiScopeClaims_ApiScopeId",
                table: "ApiScopeClaims");

            migrationBuilder.DropColumn(
                name: "ApiScopeId",
                table: "ApiScopeClaims");

            migrationBuilder.AlterColumn<int>(
                name: "ScopeId",
                table: "ApiScopeClaims",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApiScopeClaims_ScopeId",
                table: "ApiScopeClaims",
                column: "ScopeId");
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApiScopeClaims_ScopeId",
                table: "ApiScopeClaims");

            migrationBuilder.AlterColumn<int>(
                name: "ScopeId",
                table: "ApiScopeClaims",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "ApiScopeId",
                table: "ApiScopeClaims",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ApiScopeClaims_ApiScopeId",
                table: "ApiScopeClaims",
                column: "ApiScopeId");
        }
    }
}
