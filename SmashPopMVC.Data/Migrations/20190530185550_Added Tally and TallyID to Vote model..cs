using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SmashPopMVC.Data.Migrations
{
    public partial class AddedTallyandTallyIDtoVotemodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Tally_TallyID",
                table: "Votes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tally",
                table: "Tally");

            migrationBuilder.RenameTable(
                name: "Tally",
                newName: "Tallies");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tallies",
                table: "Tallies",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Tallies_TallyID",
                table: "Votes",
                column: "TallyID",
                principalTable: "Tallies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Tallies_TallyID",
                table: "Votes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tallies",
                table: "Tallies");

            migrationBuilder.RenameTable(
                name: "Tallies",
                newName: "Tally");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tally",
                table: "Tally",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Tally_TallyID",
                table: "Votes",
                column: "TallyID",
                principalTable: "Tally",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
