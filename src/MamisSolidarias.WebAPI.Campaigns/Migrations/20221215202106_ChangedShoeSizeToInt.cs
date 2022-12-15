using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MamisSolidarias.WebAPI.Campaigns.Migrations
{
    /// <inheritdoc />
    public partial class ChangedShoeSizeToInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "ALTER TABLE \"JuntosParticipants\" ALTER COLUMN \"ShoeSize\" TYPE integer USING \"ShoeSize\"::integer;"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "ALTER TABLE \"JuntosParticipants\" ALTER COLUMN \"ShoeSize\" TYPE varchar(50) USING \"ShoeSize\"::varchar(50);"
            );
        }
    }
}
