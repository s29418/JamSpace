using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JamSpace.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixUserFollowsCheckConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_UserFollows_NoSelfFollow",
                table: "UserFollows");

            migrationBuilder.AddCheckConstraint(
                name: "CK_UserFollows_NoSelfFollow",
                table: "UserFollows",
                sql: "[FollowerId] <> [FolloweeId]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_UserFollows_NoSelfFollow",
                table: "UserFollows");

            migrationBuilder.AddCheckConstraint(
                name: "CK_UserFollows_NoSelfFollow",
                table: "UserFollows",
                sql: "[FollowerId] <> [FollowedId]");
        }
    }
}
