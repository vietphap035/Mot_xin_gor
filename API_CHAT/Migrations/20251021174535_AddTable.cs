using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_CHAT.Migrations
{
    /// <inheritdoc />
    public partial class AddTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    RId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoomName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.RId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UId);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    MId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.MId);
                    table.ForeignKey(
                        name: "FK_Messages_Rooms_RId",
                        column: x => x.RId,
                        principalTable: "Rooms",
                        principalColumn: "RId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Messages_Users_UId",
                        column: x => x.UId,
                        principalTable: "Users",
                        principalColumn: "UId");
                });

            migrationBuilder.CreateTable(
                name: "UserRooms",
                columns: table => new
                {
                    UId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRooms", x => new { x.UId, x.RId });
                    table.ForeignKey(
                        name: "FK_UserRooms_Rooms_RId",
                        column: x => x.RId,
                        principalTable: "Rooms",
                        principalColumn: "RId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRooms_Users_UId",
                        column: x => x.UId,
                        principalTable: "Users",
                        principalColumn: "UId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_RoomId_SendTime",
                table: "Messages",
                columns: new[] { "RId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_UId",
                table: "Messages",
                column: "UId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRooms_RId",
                table: "UserRooms",
                column: "RId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "UserRooms");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
