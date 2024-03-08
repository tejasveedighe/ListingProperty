using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ListingProperty.Migrations
{
    /// <inheritdoc />
    public partial class OwnedPropertyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LpOwnedProperties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyId = table.Column<int>(type: "int", nullable: false),
                    BuyerUserId = table.Column<int>(type: "int", nullable: false),
                    OfferId = table.Column<int>(type: "int", nullable: false),
                    TransactionPaymentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LpOwnedProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LpOwnedProperties_LpPayments_TransactionPaymentId",
                        column: x => x.TransactionPaymentId,
                        principalTable: "LpPayments",
                        principalColumn: "PaymentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LpOwnedProperties_LpPropertyOffers_OfferId",
                        column: x => x.OfferId,
                        principalTable: "LpPropertyOffers",
                        principalColumn: "OfferId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LpOwnedProperties_LpUser_BuyerUserId",
                        column: x => x.BuyerUserId,
                        principalTable: "LpUser",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LpOwnedProperties_lpProperty_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "lpProperty",
                        principalColumn: "PropertyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LpOwnedProperties_BuyerUserId",
                table: "LpOwnedProperties",
                column: "BuyerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LpOwnedProperties_OfferId",
                table: "LpOwnedProperties",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_LpOwnedProperties_PropertyId",
                table: "LpOwnedProperties",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_LpOwnedProperties_TransactionPaymentId",
                table: "LpOwnedProperties",
                column: "TransactionPaymentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LpOwnedProperties");
        }
    }
}
