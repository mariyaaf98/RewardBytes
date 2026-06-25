using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BytesRewards.Service.Migrations
{
    /// <inheritdoc />
    public partial class DesignationBugFixMigration : Migration
    {
        // Well-known Guid used as the "Unassigned" seed designation
        private static readonly Guid UnassignedDesignationId =
            new Guid("00000000-0000-0000-0000-000000000001");

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Designation",
                table: "Users");

            migrationBuilder.AddColumn<Guid>(
                name: "DesignationId",
                table: "Users",
                type: "uuid",
                nullable: false,
                defaultValue: UnassignedDesignationId);

            migrationBuilder.CreateTable(
                name: "Designations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Designations", x => x.Id);
                });

            // Seed a fallback designation so existing users have a valid FK target
            migrationBuilder.InsertData(
                table: "Designations",
                columns: new[] { "Id", "Name", "Description", "IsActive", "CreatedAt" },
                values: new object[]
                {
                    UnassignedDesignationId,
                    "Unassigned",
                    "Default designation for existing users. Please update.",
                    true,
                    DateTime.UtcNow
                });

            // Point all existing users to the seed designation
            migrationBuilder.Sql(
                $"UPDATE \"Users\" SET \"DesignationId\" = '{UnassignedDesignationId}'");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DesignationId",
                table: "Users",
                column: "DesignationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Designations_DesignationId",
                table: "Users",
                column: "DesignationId",
                principalTable: "Designations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Designations_DesignationId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Designations");

            migrationBuilder.DropIndex(
                name: "IX_Users_DesignationId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DesignationId",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "Designation",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
