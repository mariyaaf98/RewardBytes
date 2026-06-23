using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BytesRewards.Service.Migrations
{
    /// <inheritdoc />
    public partial class RewardItemMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "RewardItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProductCode",
                table: "RewardItems",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "RewardItems");

            migrationBuilder.DropColumn(
                name: "ProductCode",
                table: "RewardItems");
        }
    }
}
