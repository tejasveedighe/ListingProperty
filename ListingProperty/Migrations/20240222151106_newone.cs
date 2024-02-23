using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ListingProperty.Migrations
{
    /// <inheritdoc />
    public partial class newone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LpContactApproval",
                columns: table => new
                {
                    ContactApprovalId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PropertyId = table.Column<int>(type: "int", nullable: false),
                    ApprovalStatus = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LpContactApproval", x => x.ContactApprovalId);
                    table.ForeignKey(
                        name: "FK_LpContactApproval_LpUser_UserId",
                        column: x => x.UserId,
                        principalTable: "LpUser",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LpContactApproval_lpProperty_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "lpProperty",
                        principalColumn: "PropertyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LpContactApproval_PropertyId",
                table: "LpContactApproval",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_LpContactApproval_UserId",
                table: "LpContactApproval",
                column: "UserId");
        }
        


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LpContactApproval");
        }
    }
}
