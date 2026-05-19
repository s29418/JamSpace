using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JamSpace.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SpotifyPlaylistAddedToPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SpotifyPlaylistEmbedUrl",
                table: "Post",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpotifyPlaylistExternalUrl",
                table: "Post",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpotifyPlaylistTitle",
                table: "Post",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpotifyPlaylistEmbedUrl",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "SpotifyPlaylistExternalUrl",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "SpotifyPlaylistTitle",
                table: "Post");
        }
    }
}
