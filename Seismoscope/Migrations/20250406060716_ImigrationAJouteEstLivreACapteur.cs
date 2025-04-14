using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seismoscope.Migrations
{
    /// <inheritdoc />
    public partial class ImigrationAJouteEstLivreACapteur : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EstLivre",
                table: "Capteurs",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstLivre",
                table: "Capteurs");
        }
    }
}
