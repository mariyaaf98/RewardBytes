using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BytesRewards.Service.Migrations
{
    /// <inheritdoc />
    public partial class AddUserNameSnapshotsAndRedemptionProductName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FromUserName",
                table: "Rewards",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ToUserName",
                table: "Rewards",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "Redemptions",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FromUserName",
                table: "Appreciations",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ToUserName",
                table: "Appreciations",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromUserName",
                table: "Rewards");

            migrationBuilder.DropColumn(
                name: "ToUserName",
                table: "Rewards");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "Redemptions");

            migrationBuilder.DropColumn(
                name: "FromUserName",
                table: "Appreciations");

            migrationBuilder.DropColumn(
                name: "ToUserName",
                table: "Appreciations");
        }
    }
}
