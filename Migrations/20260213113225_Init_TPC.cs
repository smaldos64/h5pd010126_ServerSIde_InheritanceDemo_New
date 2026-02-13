using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InheritanceDemo.Migrations
{
    /// <inheritdoc />
    public partial class Init_TPC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "AnsatSequence");

            migrationBuilder.CreateSequence(
                name: "StudentSequence");

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
                name: "Ansatte",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, defaultValueSql: "NEXT VALUE FOR [AnsatSequence]"),
                    MaanedsLoen = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AfdelingId = table.Column<int>(type: "int", nullable: false),
                    Navn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Alder = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ansatte", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ansatte_Afdelinger_AfdelingId",
                        column: x => x.AfdelingId,
                        principalTable: "Afdelinger",
                        principalColumn: "AfdelingId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Studerende",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, defaultValueSql: "NEXT VALUE FOR [StudentSequence]"),
                    HoldId = table.Column<int>(type: "int", nullable: false),
                    Navn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Alder = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Studerende", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Studerende_Hold_HoldId",
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
                        name: "FK_FagStudent_Studerende_StuderendeId",
                        column: x => x.StuderendeId,
                        principalTable: "Studerende",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ansatte_AfdelingId",
                table: "Ansatte",
                column: "AfdelingId");

            migrationBuilder.CreateIndex(
                name: "IX_FagStudent_StuderendeId",
                table: "FagStudent",
                column: "StuderendeId");

            migrationBuilder.CreateIndex(
                name: "IX_Studerende_HoldId",
                table: "Studerende",
                column: "HoldId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ansatte");

            migrationBuilder.DropTable(
                name: "FagStudent");

            migrationBuilder.DropTable(
                name: "Afdelinger");

            migrationBuilder.DropTable(
                name: "Fag");

            migrationBuilder.DropTable(
                name: "Studerende");

            migrationBuilder.DropTable(
                name: "Hold");

            migrationBuilder.DropSequence(
                name: "AnsatSequence");

            migrationBuilder.DropSequence(
                name: "StudentSequence");
        }
    }
}
