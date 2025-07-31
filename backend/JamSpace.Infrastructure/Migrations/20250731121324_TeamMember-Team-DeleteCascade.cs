using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JamSpace.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TeamMemberTeamDeleteCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamMember_Team_TeamId",
                table: "TeamMember");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMember_Team_TeamId",
                table: "TeamMember",
                column: "TeamId",
                principalTable: "Team",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamMember_Team_TeamId",
                table: "TeamMember");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMember_Team_TeamId",
                table: "TeamMember",
                column: "TeamId",
                principalTable: "Team",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
