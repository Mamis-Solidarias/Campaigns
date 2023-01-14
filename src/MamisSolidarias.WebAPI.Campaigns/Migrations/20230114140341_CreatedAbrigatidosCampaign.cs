using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MamisSolidarias.WebAPI.Campaigns.Migrations
{
    /// <inheritdoc />
    public partial class CreatedAbrigatidosCampaign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AbrigaditosCampaigns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Provider = table.Column<string>(type: "text", nullable: true),
                    Edition = table.Column<string>(type: "text", nullable: false),
                    CommunityId = table.Column<string>(type: "text", nullable: false),
                    FundraiserGoal = table.Column<decimal>(type: "numeric(2)", precision: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbrigaditosCampaigns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbrigaditosDonations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbrigaditosDonations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbrigaditosDonations_AbrigaditosCampaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "AbrigaditosCampaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbrigaditosParticipants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShirtSize = table.Column<string>(type: "text", nullable: false),
                    CampaignId = table.Column<int>(type: "integer", nullable: false),
                    BeneficiaryId = table.Column<int>(type: "integer", nullable: false),
                    BeneficiaryName = table.Column<string>(type: "text", nullable: false),
                    BeneficiaryGender = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbrigaditosParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbrigaditosParticipants_AbrigaditosCampaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "AbrigaditosCampaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AbrigaditosCampaigns_Edition_CommunityId",
                table: "AbrigaditosCampaigns",
                columns: new[] { "Edition", "CommunityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbrigaditosDonations_CampaignId",
                table: "AbrigaditosDonations",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_AbrigaditosParticipants_BeneficiaryName",
                table: "AbrigaditosParticipants",
                column: "BeneficiaryName");

            migrationBuilder.CreateIndex(
                name: "IX_AbrigaditosParticipants_CampaignId",
                table: "AbrigaditosParticipants",
                column: "CampaignId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AbrigaditosDonations");

            migrationBuilder.DropTable(
                name: "AbrigaditosParticipants");

            migrationBuilder.DropTable(
                name: "AbrigaditosCampaigns");
        }
    }
}
