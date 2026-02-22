using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InheritanceDemo.Migrations
{
    /// <inheritdoc />
    public partial class Nedarvning_Flere_Niveauer : Migration
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
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ansatte",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    MaanedsLoen = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AfdelingId = table.Column<int>(type: "int", nullable: false)
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
                    table.ForeignKey(
                        name: "FK_Ansatte_Personer_Id",
                        column: x => x.Id,
                        principalTable: "Personer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Studerende",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    HoldId = table.Column<int>(type: "int", nullable: false)
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
                    table.ForeignKey(
                        name: "FK_Studerende_Personer_Id",
                        column: x => x.Id,
                        principalTable: "Personer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Teachers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teachers_Personer_Id",
                        column: x => x.Id,
                        principalTable: "Personer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EUDStuderende",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Laereplads = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EUDStuderende", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EUDStuderende_Studerende_Id",
                        column: x => x.Id,
                        principalTable: "Studerende",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EUXStuderende",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Uddannelseslaengde = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EUXStuderende", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EUXStuderende_Studerende_Id",
                        column: x => x.Id,
                        principalTable: "Studerende",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateTable(
                name: "TeacherAfdeling",
                columns: table => new
                {
                    AfdelingerAfdelingId = table.Column<int>(type: "int", nullable: false),
                    TeachersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherAfdeling", x => new { x.AfdelingerAfdelingId, x.TeachersId });
                    table.ForeignKey(
                        name: "FK_TeacherAfdeling_Afdelinger_AfdelingerAfdelingId",
                        column: x => x.AfdelingerAfdelingId,
                        principalTable: "Afdelinger",
                        principalColumn: "AfdelingId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeacherAfdeling_Teachers_TeachersId",
                        column: x => x.TeachersId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeacherFag",
                columns: table => new
                {
                    FagId = table.Column<int>(type: "int", nullable: false),
                    TeachersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherFag", x => new { x.FagId, x.TeachersId });
                    table.ForeignKey(
                        name: "FK_TeacherFag_Fag_FagId",
                        column: x => x.FagId,
                        principalTable: "Fag",
                        principalColumn: "FagId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeacherFag_Teachers_TeachersId",
                        column: x => x.TeachersId,
                        principalTable: "Teachers",
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

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAfdeling_TeachersId",
                table: "TeacherAfdeling",
                column: "TeachersId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherFag_TeachersId",
                table: "TeacherFag",
                column: "TeachersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ansatte");

            migrationBuilder.DropTable(
                name: "EUDStuderende");

            migrationBuilder.DropTable(
                name: "EUXStuderende");

            migrationBuilder.DropTable(
                name: "FagStudent");

            migrationBuilder.DropTable(
                name: "TeacherAfdeling");

            migrationBuilder.DropTable(
                name: "TeacherFag");

            migrationBuilder.DropTable(
                name: "Studerende");

            migrationBuilder.DropTable(
                name: "Afdelinger");

            migrationBuilder.DropTable(
                name: "Fag");

            migrationBuilder.DropTable(
                name: "Teachers");

            migrationBuilder.DropTable(
                name: "Hold");

            migrationBuilder.DropTable(
                name: "Personer");
        }
    }
}
