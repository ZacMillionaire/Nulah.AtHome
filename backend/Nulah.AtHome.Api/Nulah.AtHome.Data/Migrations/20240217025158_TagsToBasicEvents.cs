using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nulah.AtHome.Data.Migrations
{
    /// <inheritdoc />
    public partial class TagsToBasicEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_BasicEvents_BasicEventId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_BasicEventId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "BasicEventId",
                table: "Tags");

            migrationBuilder.CreateTable(
                name: "BasicEventTag",
                columns: table => new
                {
                    EventsId = table.Column<int>(type: "integer", nullable: false),
                    TagsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasicEventTag", x => new { x.EventsId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_BasicEventTag_BasicEvents_EventsId",
                        column: x => x.EventsId,
                        principalTable: "BasicEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BasicEventTag_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BasicEventTag_TagsId",
                table: "BasicEventTag",
                column: "TagsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BasicEventTag");

            migrationBuilder.AddColumn<int>(
                name: "BasicEventId",
                table: "Tags",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_BasicEventId",
                table: "Tags",
                column: "BasicEventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_BasicEvents_BasicEventId",
                table: "Tags",
                column: "BasicEventId",
                principalTable: "BasicEvents",
                principalColumn: "Id");
        }
    }
}
