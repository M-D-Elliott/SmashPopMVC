using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SmashPopMVC.Data.Migrations
{
    public partial class Addedusercharactervotingentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Vote",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FlavorOfTheMonthID = table.Column<int>(type: "int", nullable: true),
                    LeastDifficultID = table.Column<int>(type: "int", nullable: true),
                    MostDifficultID = table.Column<int>(type: "int", nullable: false),
                    MostPowerfulID = table.Column<int>(type: "int", nullable: true),
                    VoterID = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vote", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Vote_Characters_FlavorOfTheMonthID",
                        column: x => x.FlavorOfTheMonthID,
                        principalTable: "Characters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vote_Characters_LeastDifficultID",
                        column: x => x.LeastDifficultID,
                        principalTable: "Characters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vote_Characters_MostDifficultID",
                        column: x => x.MostDifficultID,
                        principalTable: "Characters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Vote_Characters_MostPowerfulID",
                        column: x => x.MostPowerfulID,
                        principalTable: "Characters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vote_AspNetUsers_VoterID",
                        column: x => x.VoterID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vote_FlavorOfTheMonthID",
                table: "Vote",
                column: "FlavorOfTheMonthID");

            migrationBuilder.CreateIndex(
                name: "IX_Vote_LeastDifficultID",
                table: "Vote",
                column: "LeastDifficultID");

            migrationBuilder.CreateIndex(
                name: "IX_Vote_MostDifficultID",
                table: "Vote",
                column: "MostDifficultID");

            migrationBuilder.CreateIndex(
                name: "IX_Vote_MostPowerfulID",
                table: "Vote",
                column: "MostPowerfulID");

            migrationBuilder.CreateIndex(
                name: "IX_Vote_VoterID",
                table: "Vote",
                column: "VoterID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Vote");
        }
    }
}
