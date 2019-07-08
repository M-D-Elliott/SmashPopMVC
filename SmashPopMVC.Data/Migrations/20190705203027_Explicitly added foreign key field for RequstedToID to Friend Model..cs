using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SmashPopMVC.Data.Migrations
{
    public partial class ExplicitlyaddedforeignkeyfieldforRequstedToIDtoFriendModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friends_AspNetUsers_RequestedById",
                table: "Friends");

            migrationBuilder.DropForeignKey(
                name: "FK_Friends_AspNetUsers_RequestedToId",
                table: "Friends");

            migrationBuilder.RenameColumn(
                name: "RequestedToId",
                table: "Friends",
                newName: "RequestedToID");

            migrationBuilder.RenameColumn(
                name: "RequestedById",
                table: "Friends",
                newName: "RequestedByID");

            migrationBuilder.RenameIndex(
                name: "IX_Friends_RequestedToId",
                table: "Friends",
                newName: "IX_Friends_RequestedToID");

            migrationBuilder.RenameIndex(
                name: "IX_Friends_RequestedById",
                table: "Friends",
                newName: "IX_Friends_RequestedByID");

            migrationBuilder.AddForeignKey(
                name: "FK_Friends_AspNetUsers_RequestedByID",
                table: "Friends",
                column: "RequestedByID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Friends_AspNetUsers_RequestedToID",
                table: "Friends",
                column: "RequestedToID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friends_AspNetUsers_RequestedByID",
                table: "Friends");

            migrationBuilder.DropForeignKey(
                name: "FK_Friends_AspNetUsers_RequestedToID",
                table: "Friends");

            migrationBuilder.RenameColumn(
                name: "RequestedToID",
                table: "Friends",
                newName: "RequestedToId");

            migrationBuilder.RenameColumn(
                name: "RequestedByID",
                table: "Friends",
                newName: "RequestedById");

            migrationBuilder.RenameIndex(
                name: "IX_Friends_RequestedToID",
                table: "Friends",
                newName: "IX_Friends_RequestedToId");

            migrationBuilder.RenameIndex(
                name: "IX_Friends_RequestedByID",
                table: "Friends",
                newName: "IX_Friends_RequestedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Friends_AspNetUsers_RequestedById",
                table: "Friends",
                column: "RequestedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Friends_AspNetUsers_RequestedToId",
                table: "Friends",
                column: "RequestedToId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
