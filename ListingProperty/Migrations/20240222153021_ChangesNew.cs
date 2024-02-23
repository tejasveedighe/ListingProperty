using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ListingProperty.Migrations
{
    /// <inheritdoc />
    public partial class ChangesNew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ApprovalStatuss",
                table: "LpContactApproval",
                newName: "ApprovalStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ApprovalStatus",
                table: "LpContactApproval",
                newName: "ApprovalStatuss");
        }
    }
}
