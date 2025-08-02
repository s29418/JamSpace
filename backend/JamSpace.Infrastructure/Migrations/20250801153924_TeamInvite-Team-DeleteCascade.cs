using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JamSpace.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TeamInviteTeamDeleteCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamInvite_Team_TeamId",
                table: "TeamInvite");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamInvite_Team_TeamId",
                table: "TeamInvite",
                column: "TeamId",
                principalTable: "Team",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamInvite_Team_TeamId",
                table: "TeamInvite");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamInvite_Team_TeamId",
                table: "TeamInvite",
                column: "TeamId",
                principalTable: "Team",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
