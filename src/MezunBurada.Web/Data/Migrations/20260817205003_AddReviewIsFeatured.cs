using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MezunBurada.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewIsFeatured : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "Reviews",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "Reviews");
        }
    }
}
