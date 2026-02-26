using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JamSpace.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserFunctionalRoleAsInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoleTemp",
                table: "TeamMember",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"
        UPDATE [TeamMember]
        SET [RoleTemp] =
            CASE LOWER([Role])
                WHEN 'Member' THEN 0
                WHEN 'Admin'  THEN 1
                WHEN 'Leader' THEN 2
                ELSE 0
            END
    ");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "TeamMember");

            migrationBuilder.RenameColumn(
                name: "RoleTemp",
                table: "TeamMember",
                newName: "Role");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RoleTemp",
                table: "TeamMember",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "member");

            migrationBuilder.Sql(@"
        UPDATE [TeamMember]
        SET [RoleTemp] =
            CASE [Role]
                WHEN 0 THEN 'Member'
                WHEN 1 THEN 'Admin'
                WHEN 2 THEN 'Leader'
                ELSE 'Member'
            END
    ");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "TeamMember");

            migrationBuilder.RenameColumn(
                name: "RoleTemp",
                table: "TeamMember",
                newName: "Role");
        }
    }
}
