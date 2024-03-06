using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ListingProperty.Migrations
{
    /// <inheritdoc />
    public partial class PropertyOfferModalUpdate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SellerApproved",
                table: "LpPropertyOffers",
                newName: "SellerStatus");

            migrationBuilder.RenameColumn(
                name: "OfferCompleted",
                table: "LpPropertyOffers",
                newName: "OfferStatus");

            migrationBuilder.RenameColumn(
                name: "AdminApproved",
                table: "LpPropertyOffers",
                newName: "AdminStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SellerStatus",
                table: "LpPropertyOffers",
                newName: "SellerApproved");

            migrationBuilder.RenameColumn(
                name: "OfferStatus",
                table: "LpPropertyOffers",
                newName: "OfferCompleted");

            migrationBuilder.RenameColumn(
                name: "AdminStatus",
                table: "LpPropertyOffers",
                newName: "AdminApproved");
        }
    }
}
