using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ListingProperty.Migrations
{
    /// <inheritdoc />
    public partial class Amenities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AmenitiesId",
                table: "lpProperty",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Amenities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SwimmingPool = table.Column<bool>(type: "bit", nullable: false),
                    Parking = table.Column<bool>(type: "bit", nullable: false),
                    Lifts = table.Column<bool>(type: "bit", nullable: false),
                    Temple = table.Column<bool>(type: "bit", nullable: false),
                    RooftopAccess = table.Column<bool>(type: "bit", nullable: false),
                    Parks = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Amenities", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_lpProperty_AmenitiesId",
                table: "lpProperty",
                column: "AmenitiesId");

            migrationBuilder.AddForeignKey(
                name: "FK_lpProperty_Amenities_AmenitiesId",
                table: "lpProperty",
                column: "AmenitiesId",
                principalTable: "Amenities",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_lpProperty_Amenities_AmenitiesId",
                table: "lpProperty");

            migrationBuilder.DropTable(
                name: "Amenities");

            migrationBuilder.DropIndex(
                name: "IX_lpProperty_AmenitiesId",
                table: "lpProperty");

            migrationBuilder.DropColumn(
                name: "AmenitiesId",
                table: "lpProperty");
        }
    }
}
