using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIController.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    Telefono = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    Biografia = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Foto = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Escultores", x => x.EscultorId);
                });

            migrationBuilder.CreateTable(
                name: "Eventos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Lugar = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tematica = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    latitud = table.Column<double>(type: "float", nullable: true),
                    longitud = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Eventos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Esculturas",
                columns: table => new
                {
                    EsculturaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Imagenes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EscultoresID = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateOnly>(type: "date", nullable: false),
                    Tematica = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Esculturas", x => x.EsculturaId);
                    table.ForeignKey(
                        name: "FK_Esculturas_Escultores_EscultoresID",
                        column: x => x.EscultoresID,
                        principalTable: "Escultores",
                        principalColumn: "EscultorId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Esculturas_EscultoresID",
                table: "Esculturas",
                column: "EscultoresID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Esculturas");

            migrationBuilder.DropTable(
                name: "Eventos");

            migrationBuilder.DropTable(
                name: "Escultores");
        }
    }
}
