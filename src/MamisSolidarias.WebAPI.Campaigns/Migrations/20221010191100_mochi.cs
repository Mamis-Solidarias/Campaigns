using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MamisSolidarias.WebAPI.Campaigns.Migrations
{
    public partial class mochi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MochiCampaigns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Edition = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    CommunityId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MochiCampaigns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MochiParticipant",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CampaignId = table.Column<int>(type: "integer", nullable: false),
                    BeneficiaryId = table.Column<int>(type: "integer", nullable: false),
                    DonorId = table.Column<int>(type: "integer", nullable: true),
                    BeneficiaryName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DonorName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BeneficiaryGender = table.Column<int>(type: "integer", nullable: false),
                    SchoolCycle = table.Column<int>(type: "integer", nullable: true),
                    DonationType = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MochiParticipant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MochiParticipant_MochiCampaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "MochiCampaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MochiCampaigns_Edition_CommunityId",
                table: "MochiCampaigns",
                columns: new[] { "Edition", "CommunityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MochiParticipant_BeneficiaryId_DonorId_CampaignId",
                table: "MochiParticipant",
                columns: new[] { "BeneficiaryId", "DonorId", "CampaignId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MochiParticipant_CampaignId",
                table: "MochiParticipant",
                column: "CampaignId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MochiParticipant");

            migrationBuilder.DropTable(
                name: "MochiCampaigns");
        }
    }
}
