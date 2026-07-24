using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookManagement.Migrations
{
    /// <inheritdoc />
    public partial class InitUserSecret : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SavedToken",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    TokenValue = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedToken", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SavedToken");
        }
    }
}
