using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JamSpace.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NotificationEntityAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecipientUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActorUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(240)", maxLength: 240, nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ReadAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ConversationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TeamInviteId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TeamEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProjectNoteId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notification_User_ActorUserId",
                        column: x => x.ActorUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notification_User_RecipientUserId",
                        column: x => x.RecipientUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notification_ActorUserId",
                table: "Notification",
                column: "ActorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_ConversationId",
                table: "Notification",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_CreatedAt",
                table: "Notification",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_PostId",
                table: "Notification",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_ProjectId",
                table: "Notification",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_ProjectNoteId",
                table: "Notification",
                column: "ProjectNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_RecipientUserId",
                table: "Notification",
                column: "RecipientUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_RecipientUserId_CreatedAt",
                table: "Notification",
                columns: new[] { "RecipientUserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Notification_RecipientUserId_IsRead_CreatedAt",
                table: "Notification",
                columns: new[] { "RecipientUserId", "IsRead", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Notification_RecipientUserId_Type_ConversationId_IsRead",
                table: "Notification",
                columns: new[] { "RecipientUserId", "Type", "ConversationId", "IsRead" });

            migrationBuilder.CreateIndex(
                name: "IX_Notification_TeamEventId",
                table: "Notification",
                column: "TeamEventId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_TeamId",
                table: "Notification",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_TeamInviteId",
                table: "Notification",
                column: "TeamInviteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notification");
        }
    }
}
