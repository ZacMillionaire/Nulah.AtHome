using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nulah.AtHome.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeToSnakeCasing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BasicEventTag_BasicEvents_EventsId",
                table: "BasicEventTag");

            migrationBuilder.DropForeignKey(
                name: "FK_BasicEventTag_Tags_TagsId",
                table: "BasicEventTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tags",
                table: "Tags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BasicEventTag",
                table: "BasicEventTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BasicEvents",
                table: "BasicEvents");

            migrationBuilder.RenameTable(
                name: "Tags",
                newName: "tags");

            migrationBuilder.RenameTable(
                name: "BasicEventTag",
                newName: "basic_event_tag");

            migrationBuilder.RenameTable(
                name: "BasicEvents",
                newName: "basic_events");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "tags",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "tags",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedUtc",
                table: "tags",
                newName: "updated_utc");

            migrationBuilder.RenameColumn(
                name: "CreatedUtc",
                table: "tags",
                newName: "created_utc");

            migrationBuilder.RenameColumn(
                name: "TagsId",
                table: "basic_event_tag",
                newName: "tags_id");

            migrationBuilder.RenameColumn(
                name: "EventsId",
                table: "basic_event_tag",
                newName: "events_id");

            migrationBuilder.RenameIndex(
                name: "IX_BasicEventTag_TagsId",
                table: "basic_event_tag",
                newName: "ix_basic_event_tag_tags_id");

            migrationBuilder.RenameColumn(
                name: "Start",
                table: "basic_events",
                newName: "start");

            migrationBuilder.RenameColumn(
                name: "End",
                table: "basic_events",
                newName: "end");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "basic_events",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "basic_events",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedUtc",
                table: "basic_events",
                newName: "updated_utc");

            migrationBuilder.RenameColumn(
                name: "CreatedUtc",
                table: "basic_events",
                newName: "created_utc");

            migrationBuilder.AddPrimaryKey(
                name: "pk_tags",
                table: "tags",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_basic_event_tag",
                table: "basic_event_tag",
                columns: new[] { "events_id", "tags_id" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_basic_events",
                table: "basic_events",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_basic_event_tag_basic_events_events_id",
                table: "basic_event_tag",
                column: "events_id",
                principalTable: "basic_events",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_basic_event_tag_tags_tags_id",
                table: "basic_event_tag",
                column: "tags_id",
                principalTable: "tags",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_basic_event_tag_basic_events_events_id",
                table: "basic_event_tag");

            migrationBuilder.DropForeignKey(
                name: "fk_basic_event_tag_tags_tags_id",
                table: "basic_event_tag");

            migrationBuilder.DropPrimaryKey(
                name: "pk_tags",
                table: "tags");

            migrationBuilder.DropPrimaryKey(
                name: "pk_basic_events",
                table: "basic_events");

            migrationBuilder.DropPrimaryKey(
                name: "pk_basic_event_tag",
                table: "basic_event_tag");

            migrationBuilder.RenameTable(
                name: "tags",
                newName: "Tags");

            migrationBuilder.RenameTable(
                name: "basic_events",
                newName: "BasicEvents");

            migrationBuilder.RenameTable(
                name: "basic_event_tag",
                newName: "BasicEventTag");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Tags",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Tags",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_utc",
                table: "Tags",
                newName: "UpdatedUtc");

            migrationBuilder.RenameColumn(
                name: "created_utc",
                table: "Tags",
                newName: "CreatedUtc");

            migrationBuilder.RenameColumn(
                name: "start",
                table: "BasicEvents",
                newName: "Start");

            migrationBuilder.RenameColumn(
                name: "end",
                table: "BasicEvents",
                newName: "End");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "BasicEvents",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "BasicEvents",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_utc",
                table: "BasicEvents",
                newName: "UpdatedUtc");

            migrationBuilder.RenameColumn(
                name: "created_utc",
                table: "BasicEvents",
                newName: "CreatedUtc");

            migrationBuilder.RenameColumn(
                name: "tags_id",
                table: "BasicEventTag",
                newName: "TagsId");

            migrationBuilder.RenameColumn(
                name: "events_id",
                table: "BasicEventTag",
                newName: "EventsId");

            migrationBuilder.RenameIndex(
                name: "ix_basic_event_tag_tags_id",
                table: "BasicEventTag",
                newName: "IX_BasicEventTag_TagsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tags",
                table: "Tags",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BasicEvents",
                table: "BasicEvents",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BasicEventTag",
                table: "BasicEventTag",
                columns: new[] { "EventsId", "TagsId" });

            migrationBuilder.AddForeignKey(
                name: "FK_BasicEventTag_BasicEvents_EventsId",
                table: "BasicEventTag",
                column: "EventsId",
                principalTable: "BasicEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BasicEventTag_Tags_TagsId",
                table: "BasicEventTag",
                column: "TagsId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
