using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
#pragma warning disable CS8981

#nullable disable

namespace MamisSolidarias.WebAPI.Campaigns.Migrations
{
    public partial class juntos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JuntosCampaigns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Provider = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Edition = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    CommunityId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    FundraiserGoal = table.Column<decimal>(type: "numeric(2)", precision: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JuntosCampaigns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JuntosParticipants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Gender = table.Column<int>(type: "integer", nullable: false),
                    ShoeSize = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    State = table.Column<int>(type: "integer", nullable: false),
                    CampaignId = table.Column<int>(type: "integer", nullable: false),
                    DonorId = table.Column<int>(type: "integer", nullable: true),
                    DonorName = table.Column<string>(type: "text", nullable: true),
                    DonationType = table.Column<int>(type: "integer", nullable: true),
                    DonationDropOffPoint = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JuntosParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JuntosParticipants_JuntosCampaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "JuntosCampaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JuntosCampaigns_Edition_CommunityId",
                table: "JuntosCampaigns",
                columns: new[] { "Edition", "CommunityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JuntosParticipants_CampaignId",
                table: "JuntosParticipants",
                column: "CampaignId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JuntosParticipants");

            migrationBuilder.DropTable(
                name: "JuntosCampaigns");
        }
    }
}
