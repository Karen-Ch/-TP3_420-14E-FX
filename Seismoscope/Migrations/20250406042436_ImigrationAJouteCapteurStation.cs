using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seismoscope.Migrations
{
    /// <inheritdoc />
    public partial class ImigrationAJouteCapteurStation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Stations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", nullable: false),
                    Localisation = table.Column<string>(type: "TEXT", nullable: false),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Responsable = table.Column<string>(type: "TEXT", nullable: false),
                    DateInstallation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Etat = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Prenom = table.Column<string>(type: "TEXT", nullable: false),
                    Nom = table.Column<string>(type: "TEXT", nullable: false),
                    NomUtilisateur = table.Column<string>(type: "TEXT", nullable: false),
                    MotDePasse = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<string>(type: "TEXT", nullable: false),
                    StationId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Capteurs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Statut = table.Column<string>(type: "TEXT", nullable: false),
                    FrequenceCollecte = table.Column<double>(type: "REAL", nullable: false),
                    SeuilAlerte = table.Column<double>(type: "REAL", nullable: false),
                    DateInstallation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EstDesactive = table.Column<bool>(type: "INTEGER", nullable: false),
                    StationId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Capteurs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Capteurs_Stations_StationId",
                        column: x => x.StationId,
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Capteurs_StationId",
                table: "Capteurs",
                column: "StationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Capteurs");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Stations");
        }
    }
}
