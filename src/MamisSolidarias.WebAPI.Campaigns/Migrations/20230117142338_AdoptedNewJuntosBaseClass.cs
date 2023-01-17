using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MamisSolidarias.WebAPI.Campaigns.Migrations
{
    /// <inheritdoc />
    public partial class AdoptedNewJuntosBaseClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DonationDropOffPoint",
                table: "JuntosParticipants");

            migrationBuilder.DropColumn(
                name: "DonationType",
                table: "JuntosParticipants");

            migrationBuilder.DropColumn(
                name: "DonorId",
                table: "JuntosParticipants");

            migrationBuilder.DropColumn(
                name: "DonorName",
                table: "JuntosParticipants");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "JuntosParticipants");

            migrationBuilder.DropColumn(
                name: "State",
                table: "JuntosParticipants");

            migrationBuilder.AddColumn<string>(
                name: "BeneficiaryGender",
                table: "JuntosParticipants",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BeneficiaryName",
                table: "JuntosParticipants",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Provider",
                table: "JuntosCampaigns",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Edition",
                table: "JuntosCampaigns",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "JuntosCampaigns",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CommunityId",
                table: "JuntosCampaigns",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.CreateIndex(
                name: "IX_JuntosParticipants_BeneficiaryName",
                table: "JuntosParticipants",
                column: "BeneficiaryName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JuntosParticipants_BeneficiaryName",
                table: "JuntosParticipants");

            migrationBuilder.DropColumn(
                name: "BeneficiaryGender",
                table: "JuntosParticipants");

            migrationBuilder.DropColumn(
                name: "BeneficiaryName",
                table: "JuntosParticipants");

            migrationBuilder.AddColumn<string>(
                name: "DonationDropOffPoint",
                table: "JuntosParticipants",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DonationType",
                table: "JuntosParticipants",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DonorId",
                table: "JuntosParticipants",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DonorName",
                table: "JuntosParticipants",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "JuntosParticipants",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "JuntosParticipants",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Provider",
                table: "JuntosCampaigns",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Edition",
                table: "JuntosCampaigns",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "JuntosCampaigns",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CommunityId",
                table: "JuntosCampaigns",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
