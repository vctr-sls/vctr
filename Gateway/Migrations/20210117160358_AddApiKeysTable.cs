using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Gateway.Migrations
{
    public partial class AddApiKeysTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                 name: "ApiKeys",
                 columns: table => new
                 {
                     Guid = table.Column<Guid>(type: "uuid", nullable: false),
                     UserGuid = table.Column<Guid>(type: "uuid", nullable: true),
                     KeyHash = table.Column<string>(type: "text", nullable: true),
                     LastAccess = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                     AccessCount = table.Column<int>(type: "integer", nullable: false),
                     Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                 },
                 constraints: table =>
                 {
                     table.PrimaryKey("PK_ApiKeys", x => x.Guid);
                     table.ForeignKey(
                         name: "FK_ApiKeys_Users_UserGuid",
                         column: x => x.UserGuid,
                         principalTable: "Users",
                         principalColumn: "Guid",
                         onDelete: ReferentialAction.Restrict);
                 });

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_UserGuid",
                table: "ApiKeys",
                column: "UserGuid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiKeys");
        }
    }
}
