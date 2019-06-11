using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SmashPopMVC.Data.Migrations
{
    public partial class Addedadefaultlistforvotestousermodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vote_Characters_FlavorOfTheMonthID",
                table: "Vote");

            migrationBuilder.DropForeignKey(
                name: "FK_Vote_Characters_LeastDifficultID",
                table: "Vote");

            migrationBuilder.DropForeignKey(
                name: "FK_Vote_Characters_MostDifficultID",
                table: "Vote");

            migrationBuilder.DropForeignKey(
                name: "FK_Vote_Characters_MostPowerfulID",
                table: "Vote");

            migrationBuilder.DropForeignKey(
                name: "FK_Vote_AspNetUsers_VoterID",
                table: "Vote");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vote",
                table: "Vote");

            migrationBuilder.RenameTable(
                name: "Vote",
                newName: "Votes");

            migrationBuilder.RenameIndex(
                name: "IX_Vote_VoterID",
                table: "Votes",
                newName: "IX_Votes_VoterID");

            migrationBuilder.RenameIndex(
                name: "IX_Vote_MostPowerfulID",
                table: "Votes",
                newName: "IX_Votes_MostPowerfulID");

            migrationBuilder.RenameIndex(
                name: "IX_Vote_MostDifficultID",
                table: "Votes",
                newName: "IX_Votes_MostDifficultID");

            migrationBuilder.RenameIndex(
                name: "IX_Vote_LeastDifficultID",
                table: "Votes",
                newName: "IX_Votes_LeastDifficultID");

            migrationBuilder.RenameIndex(
                name: "IX_Vote_FlavorOfTheMonthID",
                table: "Votes",
                newName: "IX_Votes_FlavorOfTheMonthID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Votes",
                table: "Votes",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Characters_FlavorOfTheMonthID",
                table: "Votes",
                column: "FlavorOfTheMonthID",
                principalTable: "Characters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Characters_LeastDifficultID",
                table: "Votes",
                column: "LeastDifficultID",
                principalTable: "Characters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Characters_MostDifficultID",
                table: "Votes",
                column: "MostDifficultID",
                principalTable: "Characters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Characters_MostPowerfulID",
                table: "Votes",
                column: "MostPowerfulID",
                principalTable: "Characters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_AspNetUsers_VoterID",
                table: "Votes",
                column: "VoterID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Characters_FlavorOfTheMonthID",
                table: "Votes");

            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Characters_LeastDifficultID",
                table: "Votes");

            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Characters_MostDifficultID",
                table: "Votes");

            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Characters_MostPowerfulID",
                table: "Votes");

            migrationBuilder.DropForeignKey(
                name: "FK_Votes_AspNetUsers_VoterID",
                table: "Votes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Votes",
                table: "Votes");

            migrationBuilder.RenameTable(
                name: "Votes",
                newName: "Vote");

            migrationBuilder.RenameIndex(
                name: "IX_Votes_VoterID",
                table: "Vote",
                newName: "IX_Vote_VoterID");

            migrationBuilder.RenameIndex(
                name: "IX_Votes_MostPowerfulID",
                table: "Vote",
                newName: "IX_Vote_MostPowerfulID");

            migrationBuilder.RenameIndex(
                name: "IX_Votes_MostDifficultID",
                table: "Vote",
                newName: "IX_Vote_MostDifficultID");

            migrationBuilder.RenameIndex(
                name: "IX_Votes_LeastDifficultID",
                table: "Vote",
                newName: "IX_Vote_LeastDifficultID");

            migrationBuilder.RenameIndex(
                name: "IX_Votes_FlavorOfTheMonthID",
                table: "Vote",
                newName: "IX_Vote_FlavorOfTheMonthID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vote",
                table: "Vote",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Vote_Characters_FlavorOfTheMonthID",
                table: "Vote",
                column: "FlavorOfTheMonthID",
                principalTable: "Characters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vote_Characters_LeastDifficultID",
                table: "Vote",
                column: "LeastDifficultID",
                principalTable: "Characters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vote_Characters_MostDifficultID",
                table: "Vote",
                column: "MostDifficultID",
                principalTable: "Characters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vote_Characters_MostPowerfulID",
                table: "Vote",
                column: "MostPowerfulID",
                principalTable: "Characters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vote_AspNetUsers_VoterID",
                table: "Vote",
                column: "VoterID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
