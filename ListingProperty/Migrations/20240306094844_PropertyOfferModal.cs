using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ListingProperty.Migrations
{
    /// <inheritdoc />
    public partial class PropertyOfferModal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LpPropertyOffers",
                columns: table => new
                {
                    OfferId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfferPrice = table.Column<int>(type: "int", nullable: false),
                    OfferText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OfferLastDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SellerApproved = table.Column<bool>(type: "bit", nullable: false),
                    AdminApproved = table.Column<bool>(type: "bit", nullable: false),
                    OfferCompleted = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LpPropertyOffers", x => x.OfferId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LpPropertyOffers");
        }
    }
}
