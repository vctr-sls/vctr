using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Gateway.Migrations
{
    public partial class Initialization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    MailAddress = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    Permissions = table.Column<int>(type: "integer", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Guid);
                });

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

            migrationBuilder.CreateTable(
                name: "Links",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uuid", nullable: false),
                    Ident = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Destination = table.Column<string>(type: "character varying(10240)", maxLength: 10240, nullable: true),
                    CreatorGuid = table.Column<Guid>(type: "uuid", nullable: true),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    PermanentRedirect = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    LastAccess = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    AccessCount = table.Column<int>(type: "integer", nullable: false),
                    UniqueAccessCount = table.Column<int>(type: "integer", nullable: false),
                    TotalAccessLimit = table.Column<int>(type: "integer", nullable: false),
                    Expires = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Links", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_Links_Users_CreatorGuid",
                        column: x => x.CreatorGuid,
                        principalTable: "Users",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Accesses",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "uuid", nullable: false),
                    LinkGuid = table.Column<Guid>(type: "uuid", nullable: true),
                    SourceIPHash = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accesses", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_Accesses_Links_LinkGuid",
                        column: x => x.LinkGuid,
                        principalTable: "Links",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accesses_LinkGuid",
                table: "Accesses",
                column: "LinkGuid");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_UserGuid",
                table: "ApiKeys",
                column: "UserGuid");

            migrationBuilder.CreateIndex(
                name: "IX_Links_CreatorGuid",
                table: "Links",
                column: "CreatorGuid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accesses");

            migrationBuilder.DropTable(
                name: "ApiKeys");

            migrationBuilder.DropTable(
                name: "Links");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
