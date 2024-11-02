using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIController.Migrations
{
    /// <inheritdoc />
    public partial class AgregadoVotacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CantVotaciones",
                table: "Esculturas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "PromedioVotos",
                table: "Esculturas",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Votos",
                table: "Esculturas",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CantVotaciones",
                table: "Esculturas");

            migrationBuilder.DropColumn(
                name: "PromedioVotos",
                table: "Esculturas");

            migrationBuilder.DropColumn(
                name: "Votos",
                table: "Esculturas");
        }
    }
}
