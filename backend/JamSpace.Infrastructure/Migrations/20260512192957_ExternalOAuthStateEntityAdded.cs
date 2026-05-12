using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JamSpace.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExternalOAuthStateEntityAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExternalOAuthState",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Provider = table.Column<int>(type: "int", nullable: false),
                    State = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CodeVerifier = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ReturnUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ConsumedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalOAuthState", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalOAuthState_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExternalOAuthState_ExpiresAt",
                table: "ExternalOAuthState",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalOAuthState_Provider_State",
                table: "ExternalOAuthState",
                columns: new[] { "Provider", "State" });

            migrationBuilder.CreateIndex(
                name: "IX_ExternalOAuthState_State",
                table: "ExternalOAuthState",
                column: "State",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExternalOAuthState_UserId",
                table: "ExternalOAuthState",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExternalOAuthState");
        }
    }
}
