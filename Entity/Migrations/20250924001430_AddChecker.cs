using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entity.Migrations
{
    /// <inheritdoc />
    public partial class AddChecker : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Verification_User_UserId",
                schema: "System",
                table: "Verification");

            migrationBuilder.RenameColumn(
                name: "UserId",
                schema: "System",
                table: "Verification",
                newName: "CheckerId");

            migrationBuilder.RenameIndex(
                name: "IX_Verification_UserId",
                schema: "System",
                table: "Verification",
                newName: "IX_Verification_CheckerId");

            migrationBuilder.CreateTable(
                name: "Checker",
                schema: "System",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Checker", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Checker_Branch_BranchId",
                        column: x => x.BranchId,
                        principalSchema: "System",
                        principalTable: "Branch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Checker_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Checker_BranchId",
                schema: "System",
                table: "Checker",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Checker_UserId",
                schema: "System",
                table: "Checker",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Verification_Checker_CheckerId",
                schema: "System",
                table: "Verification",
                column: "CheckerId",
                principalSchema: "System",
                principalTable: "Checker",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Verification_Checker_CheckerId",
                schema: "System",
                table: "Verification");

            migrationBuilder.DropTable(
                name: "Checker",
                schema: "System");

            migrationBuilder.RenameColumn(
                name: "CheckerId",
                schema: "System",
                table: "Verification",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Verification_CheckerId",
                schema: "System",
                table: "Verification",
                newName: "IX_Verification_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Verification_User_UserId",
                schema: "System",
                table: "Verification",
                column: "UserId",
                principalSchema: "Security",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
