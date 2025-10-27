using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entity.Migrations
{
    /// <inheritdoc />
    public partial class OneToOneUserPerson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_User_PersonId",
                schema: "Security",
                table: "User");

            migrationBuilder.CreateIndex(
                name: "IX_User_PersonId",
                schema: "Security",
                table: "User",
                column: "PersonId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_User_PersonId",
                schema: "Security",
                table: "User");

            migrationBuilder.CreateIndex(
                name: "IX_User_PersonId",
                schema: "Security",
                table: "User",
                column: "PersonId");
        }
    }
}
