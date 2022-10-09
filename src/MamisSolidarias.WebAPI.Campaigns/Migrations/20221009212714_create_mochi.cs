using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MamisSolidarias.WebAPI.Campaigns.Migrations
{
    public partial class create_mochi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Edition = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    CommunityId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
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
                    DonorName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BeneficiaryGender = table.Column<int>(type: "integer", nullable: false),
                    SchoolCycle = table.Column<int>(type: "integer", nullable: true),
                    DonationType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MochiParticipant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MochiParticipant_Users_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MochiParticipant_BeneficiaryId_DonorId_CampaignId",
                table: "MochiParticipant",
                columns: new[] { "BeneficiaryId", "DonorId", "CampaignId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MochiParticipant_CampaignId",
                table: "MochiParticipant",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Edition_CommunityId",
                table: "Users",
                columns: new[] { "Edition", "CommunityId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MochiParticipant");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
