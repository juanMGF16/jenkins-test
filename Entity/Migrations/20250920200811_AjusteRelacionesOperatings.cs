using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entity.Migrations
{
    /// <inheritdoc />
    public partial class AjusteRelacionesOperatings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Operating_OperatingGroup_OperationalGroupId",
                schema: "System",
                table: "Operating");

            migrationBuilder.DropForeignKey(
                name: "FK_Operating_User_UserId",
                schema: "System",
                table: "Operating");

            migrationBuilder.DropIndex(
                name: "IX_OperatingGroup_UserId",
                schema: "System",
                table: "OperatingGroup");

            migrationBuilder.AlterColumn<int>(
                name: "OperationalGroupId",
                schema: "System",
                table: "Operating",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_OperatingGroup_UserId",
                schema: "System",
                table: "OperatingGroup",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Operating_OperatingGroup_OperationalGroupId",
                schema: "System",
                table: "Operating",
                column: "OperationalGroupId",
                principalSchema: "System",
                principalTable: "OperatingGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Operating_User_UserId",
                schema: "System",
                table: "Operating",
                column: "UserId",
                principalSchema: "Security",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Operating_OperatingGroup_OperationalGroupId",
                schema: "System",
                table: "Operating");

            migrationBuilder.DropForeignKey(
                name: "FK_Operating_User_UserId",
                schema: "System",
                table: "Operating");

            migrationBuilder.DropIndex(
                name: "IX_OperatingGroup_UserId",
                schema: "System",
                table: "OperatingGroup");

            migrationBuilder.AlterColumn<int>(
                name: "OperationalGroupId",
                schema: "System",
                table: "Operating",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OperatingGroup_UserId",
                schema: "System",
                table: "OperatingGroup",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Operating_OperatingGroup_OperationalGroupId",
                schema: "System",
                table: "Operating",
                column: "OperationalGroupId",
                principalSchema: "System",
                principalTable: "OperatingGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Operating_User_UserId",
                schema: "System",
                table: "Operating",
                column: "UserId",
                principalSchema: "Security",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
