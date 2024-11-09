using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIController.Migrations
{
    /// <inheritdoc />
    public partial class EdicionYVotos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Votos",
                table: "Esculturas",
                newName: "EdicionAño");

            migrationBuilder.AddColumn<int>(
                name: "EdicionAño",
                table: "Eventos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EdicionAño",
                table: "Escultores",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Edicion",
                columns: table => new
                {
                    Año = table.Column<int>(type: "int", nullable: false),
                    FechaInicio = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaFin = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Edicion", x => x.Año);
                });

            migrationBuilder.CreateTable(
                name: "Votos",
                columns: table => new
                {
                    VotoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UrserId = table.Column<int>(type: "int", nullable: false),
                    EsculturaId = table.Column<int>(type: "int", nullable: false),
                    Puntuacion = table.Column<float>(type: "real", nullable: false),
                    FechaCreacion = table.Column<DateOnly>(type: "date", nullable: false),
                    EsculturasEsculturaId = table.Column<int>(type: "int", nullable: true),
                    MyUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votos", x => x.VotoId);
                    table.ForeignKey(
                        name: "FK_Votos_Esculturas_EsculturasEsculturaId",
                        column: x => x.EsculturasEsculturaId,
                        principalTable: "Esculturas",
                        principalColumn: "EsculturaId");
                    table.ForeignKey(
                        name: "FK_Votos_Users_MyUserId",
                        column: x => x.MyUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Eventos_EdicionAño",
                table: "Eventos",
                column: "EdicionAño");

            migrationBuilder.CreateIndex(
                name: "IX_Esculturas_EdicionAño",
                table: "Esculturas",
                column: "EdicionAño");

            migrationBuilder.CreateIndex(
                name: "IX_Escultores_EdicionAño",
                table: "Escultores",
                column: "EdicionAño");

            migrationBuilder.CreateIndex(
                name: "IX_Votos_EsculturasEsculturaId",
                table: "Votos",
                column: "EsculturasEsculturaId");

            migrationBuilder.CreateIndex(
                name: "IX_Votos_MyUserId",
                table: "Votos",
                column: "MyUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Escultores_Edicion_EdicionAño",
                table: "Escultores",
                column: "EdicionAño",
                principalTable: "Edicion",
                principalColumn: "Año",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Esculturas_Edicion_EdicionAño",
                table: "Esculturas",
                column: "EdicionAño",
                principalTable: "Edicion",
                principalColumn: "Año",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Eventos_Edicion_EdicionAño",
                table: "Eventos",
                column: "EdicionAño",
                principalTable: "Edicion",
                principalColumn: "Año",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Escultores_Edicion_EdicionAño",
                table: "Escultores");

            migrationBuilder.DropForeignKey(
                name: "FK_Esculturas_Edicion_EdicionAño",
                table: "Esculturas");

            migrationBuilder.DropForeignKey(
                name: "FK_Eventos_Edicion_EdicionAño",
                table: "Eventos");

            migrationBuilder.DropTable(
                name: "Edicion");

            migrationBuilder.DropTable(
                name: "Votos");

            migrationBuilder.DropIndex(
                name: "IX_Eventos_EdicionAño",
                table: "Eventos");

            migrationBuilder.DropIndex(
                name: "IX_Esculturas_EdicionAño",
                table: "Esculturas");

            migrationBuilder.DropIndex(
                name: "IX_Escultores_EdicionAño",
                table: "Escultores");

            migrationBuilder.DropColumn(
                name: "EdicionAño",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "EdicionAño",
                table: "Escultores");

            migrationBuilder.RenameColumn(
                name: "EdicionAño",
                table: "Esculturas",
                newName: "Votos");
        }
    }
}
