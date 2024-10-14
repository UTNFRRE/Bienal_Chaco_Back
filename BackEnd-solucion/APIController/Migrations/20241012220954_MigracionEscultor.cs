using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIController.Migrations
{
    /// <inheritdoc />
    public partial class MigracionEscultor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EscultoresEscultorId",
                table: "Esculturas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Escultores",
                columns: table => new
                {
                    EscultorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    Apellido = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    DNI = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    Pais = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    Contraseña = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    Telefono = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    Biografia = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Foto = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Escultores", x => x.EscultorId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Esculturas_EscultoresEscultorId",
                table: "Esculturas",
                column: "EscultoresEscultorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Esculturas_Escultores_EscultoresEscultorId",
                table: "Esculturas",
                column: "EscultoresEscultorId",
                principalTable: "Escultores",
                principalColumn: "EscultorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Esculturas_Escultores_EscultoresEscultorId",
                table: "Esculturas");

            migrationBuilder.DropTable(
                name: "Escultores");

            migrationBuilder.DropIndex(
                name: "IX_Esculturas_EscultoresEscultorId",
                table: "Esculturas");

            migrationBuilder.DropColumn(
                name: "EscultoresEscultorId",
                table: "Esculturas");
        }
    }
}
