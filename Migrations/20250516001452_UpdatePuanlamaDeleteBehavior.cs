using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlasTech.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePuanlamaDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Puanlamalar_Kullanicilar_KullaniciId",
                table: "Puanlamalar");

            migrationBuilder.AddForeignKey(
                name: "FK_Puanlamalar_Kullanicilar_KullaniciId",
                table: "Puanlamalar",
                column: "KullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Puanlamalar_Kullanicilar_KullaniciId",
                table: "Puanlamalar");

            migrationBuilder.AddForeignKey(
                name: "FK_Puanlamalar_Kullanicilar_KullaniciId",
                table: "Puanlamalar",
                column: "KullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id");
        }
    }
}
