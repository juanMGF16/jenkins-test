using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entity.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIventaryVerification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Verification_Inventary_InventaryId",
                schema: "System",
                table: "Verification");

            migrationBuilder.DropIndex(
                name: "IX_Verification_InventaryId",
                schema: "System",
                table: "Verification");

            migrationBuilder.CreateIndex(
                name: "IX_Verification_InventaryId",
                schema: "System",
                table: "Verification",
                column: "InventaryId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Verification_Inventary_InventaryId",
                schema: "System",
                table: "Verification",
                column: "InventaryId",
                principalSchema: "System",
                principalTable: "Inventary",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Verification_Inventary_InventaryId",
                schema: "System",
                table: "Verification");

            migrationBuilder.DropIndex(
                name: "IX_Verification_InventaryId",
                schema: "System",
                table: "Verification");

            migrationBuilder.CreateIndex(
                name: "IX_Verification_InventaryId",
                schema: "System",
                table: "Verification",
                column: "InventaryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Verification_Inventary_InventaryId",
                schema: "System",
                table: "Verification",
                column: "InventaryId",
                principalSchema: "System",
                principalTable: "Inventary",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
