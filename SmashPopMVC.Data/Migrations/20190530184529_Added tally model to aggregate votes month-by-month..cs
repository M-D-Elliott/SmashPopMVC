using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SmashPopMVC.Data.Migrations
{
    public partial class Addedtallymodeltoaggregatevotesmonthbymonth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TallyID",
                table: "Votes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Tally",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Month = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Year = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tally", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Votes_TallyID",
                table: "Votes",
                column: "TallyID");

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Tally_TallyID",
                table: "Votes",
                column: "TallyID",
                principalTable: "Tally",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Tally_TallyID",
                table: "Votes");

            migrationBuilder.DropTable(
                name: "Tally");

            migrationBuilder.DropIndex(
                name: "IX_Votes_TallyID",
                table: "Votes");

            migrationBuilder.DropColumn(
                name: "TallyID",
                table: "Votes");
        }
    }
}
