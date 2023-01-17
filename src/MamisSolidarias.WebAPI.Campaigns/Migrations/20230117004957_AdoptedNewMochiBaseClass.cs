using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MamisSolidarias.WebAPI.Campaigns.Migrations
{
    /// <inheritdoc />
    public partial class AdoptedNewMochiBaseClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DonationDropOffLocation",
                table: "MochiParticipants");

            migrationBuilder.AlterColumn<string>(
                name: "State",
                table: "MochiParticipants",
                type: "text",
                nullable: false,
                defaultValue: "MissingDonor",
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "DonorName",
                table: "MochiParticipants",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BeneficiaryName",
                table: "MochiParticipants",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "BeneficiaryGender",
                table: "MochiParticipants",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "DonationDropOffPoint",
                table: "MochiParticipants",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Provider",
                table: "MochiCampaigns",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Edition",
                table: "MochiCampaigns",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "MochiCampaigns",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CommunityId",
                table: "MochiCampaigns",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.CreateIndex(
                name: "IX_MochiParticipants_BeneficiaryName",
                table: "MochiParticipants",
                column: "BeneficiaryName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MochiParticipants_BeneficiaryName",
                table: "MochiParticipants");

            migrationBuilder.DropColumn(
                name: "DonationDropOffPoint",
                table: "MochiParticipants");

            migrationBuilder.AlterColumn<int>(
                name: "State",
                table: "MochiParticipants",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "MissingDonor");

            migrationBuilder.AlterColumn<string>(
                name: "DonorName",
                table: "MochiParticipants",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BeneficiaryName",
                table: "MochiParticipants",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "BeneficiaryGender",
                table: "MochiParticipants",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "DonationDropOffLocation",
                table: "MochiParticipants",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Provider",
                table: "MochiCampaigns",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Edition",
                table: "MochiCampaigns",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "MochiCampaigns",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CommunityId",
                table: "MochiCampaigns",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
