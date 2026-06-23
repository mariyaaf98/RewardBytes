using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BytesRewards.Service.Migrations
{
    /// <inheritdoc />
    public partial class AddBytesToReward : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Bytes",
                table: "Rewards",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bytes",
                table: "Rewards");
        }
    }
}
