using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entity.Migrations
{
    /// <inheritdoc />
    public partial class CambiosOperating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                schema: "System",
                table: "Operating",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Operating_CreatedByUserId",
                schema: "System",
                table: "Operating",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Operating_User_CreatedByUserId",
                schema: "System",
                table: "Operating",
                column: "CreatedByUserId",
                principalSchema: "Security",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Operating_User_CreatedByUserId",
                schema: "System",
                table: "Operating");

            migrationBuilder.DropIndex(
                name: "IX_Operating_CreatedByUserId",
                schema: "System",
                table: "Operating");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                schema: "System",
                table: "Operating");
        }
    }
}
