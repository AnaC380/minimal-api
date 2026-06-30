using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace minimal_api.Migrations
{
    /// <inheritdoc />
    public partial class HashSenhaSeedCorreto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Administradores",
                keyColumn: "Id",
                keyValue: 1,
                column: "Senha",
                value: "$2a$11$l7MzSKS4ovseZOv31aMz0elKKDpswork9ACnH.k6Bsx/5SFAYV7w2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Administradores",
                keyColumn: "Id",
                keyValue: 1,
                column: "Senha",
                value: "$2a$11$bvYlMkMNHYBp9XKFM7PeaeV9HKuiOlhpHUxLI0P2M0PalRdw/E3i");
        }
    }
}
