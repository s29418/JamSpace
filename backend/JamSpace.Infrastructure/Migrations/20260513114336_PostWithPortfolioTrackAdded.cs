using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JamSpace.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PostWithPortfolioTrackAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PortfolioTrackId",
                table: "Post",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Post_PortfolioTrackId",
                table: "Post",
                column: "PortfolioTrackId");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_PortfolioTrack_PortfolioTrackId",
                table: "Post",
                column: "PortfolioTrackId",
                principalTable: "PortfolioTrack",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_PortfolioTrack_PortfolioTrackId",
                table: "Post");

            migrationBuilder.DropIndex(
                name: "IX_Post_PortfolioTrackId",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "PortfolioTrackId",
                table: "Post");
        }
    }
}
