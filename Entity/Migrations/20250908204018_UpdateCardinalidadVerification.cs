using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entity.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCardinalidadVerification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Verification_InventaryId",
                schema: "System",
                table: "Verification");

            migrationBuilder.DropIndex(
                name: "IX_Verification_UserId",
                schema: "System",
                table: "Verification");

            migrationBuilder.CreateIndex(
                name: "IX_Verification_InventaryId",
                schema: "System",
                table: "Verification",
                column: "InventaryId");

            migrationBuilder.CreateIndex(
                name: "IX_Verification_UserId",
                schema: "System",
                table: "Verification",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Verification_InventaryId",
                schema: "System",
                table: "Verification");

            migrationBuilder.DropIndex(
                name: "IX_Verification_UserId",
                schema: "System",
                table: "Verification");

            migrationBuilder.CreateIndex(
                name: "IX_Verification_InventaryId",
                schema: "System",
                table: "Verification",
                column: "InventaryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Verification_UserId",
                schema: "System",
                table: "Verification",
                column: "UserId",
                unique: true);
        }
    }
}
