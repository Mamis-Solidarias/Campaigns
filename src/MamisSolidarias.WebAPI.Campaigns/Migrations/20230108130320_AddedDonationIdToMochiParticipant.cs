using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MamisSolidarias.WebAPI.Campaigns.Migrations
{
    /// <inheritdoc />
    public partial class AddedDonationIdToMochiParticipant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DonationId",
                table: "MochiParticipants",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MochiParticipants_DonationId",
                table: "MochiParticipants",
                column: "DonationId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MochiParticipants_DonationId",
                table: "MochiParticipants");

            migrationBuilder.DropColumn(
                name: "DonationId",
                table: "MochiParticipants");
        }
    }
}
