using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BytesRewards.Service.Migrations
{
    /// <inheritdoc />
    public partial class AddRewardCategoryNameSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RewardCategoryName",
                table: "Rewards",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RewardCategoryName",
                table: "Rewards");
        }
    }
}
