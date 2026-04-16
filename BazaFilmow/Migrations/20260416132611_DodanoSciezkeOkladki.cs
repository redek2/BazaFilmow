using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BazaFilmow.Migrations
{
    /// <inheritdoc />
    public partial class DodanoSciezkeOkladki : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SciezkaOkladki",
                table: "Filmy",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SciezkaOkladki",
                table: "Filmy");
        }
    }
}
