using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JamSpace.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LocationForUserNoLongerRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_User_Loc_Country", table: "User");
            migrationBuilder.DropIndex(name: "IX_User_Loc_Country_City", table: "User");
            migrationBuilder.DropIndex(name: "IX_User_Loc_US_State_City", table: "User");

            migrationBuilder.AlterColumn<string>(
                name: "Location_CountryCode",
                table: "User",
                type: "varchar(2)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(2)");

            migrationBuilder.AlterColumn<string>(
                name: "Location_City",
                table: "User",
                type: "nvarchar(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)");

            // 3) Odtworzenie indeksów
            migrationBuilder.CreateIndex(
                name: "IX_User_Loc_Country",
                table: "User",
                column: "Location_CountryCode");

            migrationBuilder.CreateIndex(
                name: "IX_User_Loc_Country_City",
                table: "User",
                columns: new[] { "Location_CountryCode", "Location_City" });

            migrationBuilder.Sql(@"
IF COL_LENGTH('[User]', 'Location_US_State') IS NOT NULL
BEGIN
    CREATE INDEX [IX_User_Loc_US_State_City]
        ON [User] ([Location_CountryCode], [Location_US_State], [Location_City]);
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_User_Loc_US_State_City' AND object_id = OBJECT_ID('[User]'))
BEGIN
    DROP INDEX [IX_User_Loc_US_State_City] ON [User];
END
");
        }
    }
}
