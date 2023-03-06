using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MamisSolidarias.WebAPI.Campaigns.Migrations
{
    /// <inheritdoc />
    public partial class AddedTotalDonationsField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalDonations",
                table: "JuntosCampaigns",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalDonations",
                table: "AbrigaditosCampaigns",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalDonations",
                table: "JuntosCampaigns");

            migrationBuilder.DropColumn(
                name: "TotalDonations",
                table: "AbrigaditosCampaigns");
        }
    }
}
