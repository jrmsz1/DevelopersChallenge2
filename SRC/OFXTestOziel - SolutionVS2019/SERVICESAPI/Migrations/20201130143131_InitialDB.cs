using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SERVICESAPI.Migrations
{
    public partial class InitialDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ACCOUNT",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FIRSTNAME = table.Column<string>(nullable: true),
                    LASTNAME = table.Column<string>(nullable: true),
                    IDENTIFICATION = table.Column<string>(nullable: true),
                    COUNTRY = table.Column<string>(nullable: true),
                    BANKID = table.Column<int>(nullable: false),
                    ACCTID = table.Column<long>(nullable: false),
                    DATECREATED = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ACCOUNT", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OFX_BATCH",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OFXHEADER = table.Column<long>(nullable: false),
                    DATA = table.Column<string>(nullable: true),
                    VERSION = table.Column<string>(nullable: true),
                    SECURITY = table.Column<string>(nullable: true),
                    ENCODING = table.Column<string>(nullable: true),
                    CHARSET = table.Column<string>(nullable: true),
                    COMPRESSION = table.Column<string>(nullable: true),
                    OLDFILEUID = table.Column<string>(nullable: true),
                    NEWFILEUID = table.Column<string>(nullable: true),
                    DT_CREATED = table.Column<DateTimeOffset>(nullable: false),
                    SONRS_STA_CODE = table.Column<int>(nullable: false),
                    SONRS_STA_SEVERITY = table.Column<string>(nullable: true),
                    DTSERVER = table.Column<DateTimeOffset>(nullable: true),
                    DTSERVER_TMZ = table.Column<string>(nullable: true),
                    LANGUAGE = table.Column<string>(nullable: true),
                    STMTTRNRS_TRNUID = table.Column<int>(nullable: false),
                    STMTTRNRS_STA_CODE = table.Column<int>(nullable: false),
                    STMTTRNRS_STA_SEVERITY = table.Column<string>(nullable: true),
                    CURDEF = table.Column<string>(nullable: true),
                    BANKID = table.Column<int>(nullable: false),
                    ACCTID = table.Column<long>(nullable: false),
                    ACCTTYPE = table.Column<string>(nullable: true),
                    DTSTART = table.Column<DateTimeOffset>(nullable: false),
                    DTSTART_TMZ = table.Column<string>(nullable: true),
                    DTEND = table.Column<DateTimeOffset>(nullable: false),
                    DTEND_TMZ = table.Column<string>(nullable: true),
                    BALAMT = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DTASOF = table.Column<DateTimeOffset>(nullable: false),
                    DTASOF_TMZ = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OFX_BATCH", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "STMTTRN",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AccountId = table.Column<long>(nullable: false),
                    BATCHId = table.Column<long>(nullable: false),
                    TRNTYPE = table.Column<string>(nullable: true),
                    DTPOSTED = table.Column<DateTimeOffset>(nullable: false),
                    DTPOSTED_TMZ = table.Column<string>(nullable: true),
                    TRNAMT = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MEMO = table.Column<string>(nullable: true),
                    DT_RECONCILIATION = table.Column<DateTimeOffset>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STMTTRN", x => x.Id);
                    table.ForeignKey(
                        name: "FK_STMTTRN_ACCOUNT_AccountId",
                        column: x => x.AccountId,
                        principalTable: "ACCOUNT",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_STMTTRN_OFX_BATCH_BATCHId",
                        column: x => x.BATCHId,
                        principalTable: "OFX_BATCH",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ACCOUNT",
                columns: new[] { "Id", "ACCTID", "BANKID", "COUNTRY", "DATECREATED", "FIRSTNAME", "IDENTIFICATION", "LASTNAME" },
                values: new object[] { 1L, 7037300576L, 341, null, new DateTimeOffset(new DateTime(2020, 11, 30, 14, 31, 31, 333, DateTimeKind.Unspecified).AddTicks(4123), new TimeSpan(0, 0, 0, 0, 0)), "Xayah", "0341-7037300576", "NIBO" });

            migrationBuilder.CreateIndex(
                name: "IX_ACCOUNT_FIRSTNAME",
                table: "ACCOUNT",
                column: "FIRSTNAME");

            migrationBuilder.CreateIndex(
                name: "IX_ACCOUNT_IDENTIFICATION",
                table: "ACCOUNT",
                column: "IDENTIFICATION");

            migrationBuilder.CreateIndex(
                name: "IX_ACCOUNT_BANKID_ACCTID",
                table: "ACCOUNT",
                columns: new[] { "BANKID", "ACCTID" });

            migrationBuilder.CreateIndex(
                name: "IX_STMTTRN_BATCHId",
                table: "STMTTRN",
                column: "BATCHId");

            migrationBuilder.CreateIndex(
                name: "IX_STMTTRN_AccountId_DTPOSTED_TRNTYPE_TRNAMT",
                table: "STMTTRN",
                columns: new[] { "AccountId", "DTPOSTED", "TRNTYPE", "TRNAMT" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "STMTTRN");

            migrationBuilder.DropTable(
                name: "ACCOUNT");

            migrationBuilder.DropTable(
                name: "OFX_BATCH");
        }
    }
}
