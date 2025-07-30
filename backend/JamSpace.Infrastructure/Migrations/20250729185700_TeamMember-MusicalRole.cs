using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JamSpace.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TeamMemberMusicalRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MusicalRole",
                table: "TeamMember",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MusicalRole",
                table: "TeamMember");
        }
    }
}
