using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ListingProperty.Migrations
{
    /// <inheritdoc />
    public partial class Amenities2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_lpProperty_Amenities_AmenitiesId",
                table: "lpProperty");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Amenities",
                table: "Amenities");

            migrationBuilder.RenameTable(
                name: "Amenities",
                newName: "LpPropertyAmenities");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LpPropertyAmenities",
                table: "LpPropertyAmenities",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_lpProperty_LpPropertyAmenities_AmenitiesId",
                table: "lpProperty",
                column: "AmenitiesId",
                principalTable: "LpPropertyAmenities",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_lpProperty_LpPropertyAmenities_AmenitiesId",
                table: "lpProperty");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LpPropertyAmenities",
                table: "LpPropertyAmenities");

            migrationBuilder.RenameTable(
                name: "LpPropertyAmenities",
                newName: "Amenities");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Amenities",
                table: "Amenities",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_lpProperty_Amenities_AmenitiesId",
                table: "lpProperty",
                column: "AmenitiesId",
                principalTable: "Amenities",
                principalColumn: "Id");
        }
    }
}
