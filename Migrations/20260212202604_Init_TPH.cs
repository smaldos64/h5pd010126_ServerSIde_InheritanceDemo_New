using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InheritanceDemo.Migrations
{
    /// <inheritdoc />
    public partial class Init_TPH : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Afdelinger",
                columns: table => new
                {
                    AfdelingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AfdelingNavn = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Afdelinger", x => x.AfdelingId);
                });

            migrationBuilder.CreateTable(
                name: "Fag",
                columns: table => new
                {
                    FagId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FagTitel = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fag", x => x.FagId);
                });

            migrationBuilder.CreateTable(
                name: "Hold",
                columns: table => new
                {
                    HoldId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoldNavn = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hold", x => x.HoldId);
                });

            migrationBuilder.CreateTable(
                name: "Personer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Navn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Alder = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PersonType = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    MaanedsLoen = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    AfdelingId = table.Column<int>(type: "int", nullable: true),
                    HoldId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Personer_Afdelinger_AfdelingId",
                        column: x => x.AfdelingId,
                        principalTable: "Afdelinger",
                        principalColumn: "AfdelingId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Personer_Hold_HoldId",
                        column: x => x.HoldId,
                        principalTable: "Hold",
                        principalColumn: "HoldId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FagStudent",
                columns: table => new
                {
                    FagId = table.Column<int>(type: "int", nullable: false),
                    StuderendeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FagStudent", x => new { x.FagId, x.StuderendeId });
                    table.ForeignKey(
                        name: "FK_FagStudent_Fag_FagId",
                        column: x => x.FagId,
                        principalTable: "Fag",
                        principalColumn: "FagId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FagStudent_Personer_StuderendeId",
                        column: x => x.StuderendeId,
                        principalTable: "Personer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FagStudent_StuderendeId",
                table: "FagStudent",
                column: "StuderendeId");

            migrationBuilder.CreateIndex(
                name: "IX_Personer_AfdelingId",
                table: "Personer",
                column: "AfdelingId");

            migrationBuilder.CreateIndex(
                name: "IX_Personer_HoldId",
                table: "Personer",
                column: "HoldId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FagStudent");

            migrationBuilder.DropTable(
                name: "Fag");

            migrationBuilder.DropTable(
                name: "Personer");

            migrationBuilder.DropTable(
                name: "Afdelinger");

            migrationBuilder.DropTable(
                name: "Hold");
        }
    }
}
