using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JamSpace.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProjectNoteEntityAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectNote",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AudioVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompletedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    StartTimeSeconds = table.Column<int>(type: "int", nullable: true),
                    EndTimeSeconds = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CompletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectNote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectNote_ProjectAudioVersion_AudioVersionId",
                        column: x => x.AudioVersionId,
                        principalTable: "ProjectAudioVersion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ProjectNote_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectNote_User_CompletedById",
                        column: x => x.CompletedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectNote_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectNote_AudioVersionId",
                table: "ProjectNote",
                column: "AudioVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectNote_CompletedById",
                table: "ProjectNote",
                column: "CompletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectNote_CreatedById",
                table: "ProjectNote",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectNote_ProjectId",
                table: "ProjectNote",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectNote_ProjectId_Status_CreatedAt",
                table: "ProjectNote",
                columns: new[] { "ProjectId", "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectNote_Status",
                table: "ProjectNote",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectNote");
        }
    }
}
