using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MamisSolidarias.WebAPI.Campaigns.Migrations
{
    public partial class mochi_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MochiParticipant_MochiCampaigns_CampaignId",
                table: "MochiParticipant");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MochiParticipant",
                table: "MochiParticipant");

            migrationBuilder.RenameTable(
                name: "MochiParticipant",
                newName: "MochiParticipants");

            migrationBuilder.RenameIndex(
                name: "IX_MochiParticipant_CampaignId",
                table: "MochiParticipants",
                newName: "IX_MochiParticipants_CampaignId");

            migrationBuilder.RenameIndex(
                name: "IX_MochiParticipant_BeneficiaryId_DonorId_CampaignId",
                table: "MochiParticipants",
                newName: "IX_MochiParticipants_BeneficiaryId_DonorId_CampaignId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MochiParticipants",
                table: "MochiParticipants",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MochiParticipants_MochiCampaigns_CampaignId",
                table: "MochiParticipants",
                column: "CampaignId",
                principalTable: "MochiCampaigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MochiParticipants_MochiCampaigns_CampaignId",
                table: "MochiParticipants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MochiParticipants",
                table: "MochiParticipants");

            migrationBuilder.RenameTable(
                name: "MochiParticipants",
                newName: "MochiParticipant");

            migrationBuilder.RenameIndex(
                name: "IX_MochiParticipants_CampaignId",
                table: "MochiParticipant",
                newName: "IX_MochiParticipant_CampaignId");

            migrationBuilder.RenameIndex(
                name: "IX_MochiParticipants_BeneficiaryId_DonorId_CampaignId",
                table: "MochiParticipant",
                newName: "IX_MochiParticipant_BeneficiaryId_DonorId_CampaignId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MochiParticipant",
                table: "MochiParticipant",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MochiParticipant_MochiCampaigns_CampaignId",
                table: "MochiParticipant",
                column: "CampaignId",
                principalTable: "MochiCampaigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
