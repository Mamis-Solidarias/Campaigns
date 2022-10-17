using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MamisSolidarias.WebAPI.Campaigns.Migrations
{
    public partial class juntos_removedPrecision : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "FundraiserGoal",
                table: "JuntosCampaigns",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(2,0)",
                oldPrecision: 2);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "FundraiserGoal",
                table: "JuntosCampaigns",
                type: "numeric(2,0)",
                precision: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");
        }
    }
}
