using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SmashPopMVC.Data.Migrations
{
    public partial class Generalizedfriends : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BecameFriendsTime",
                table: "Friends");

            migrationBuilder.DropColumn(
                name: "FriendRequestFlag",
                table: "Friends");

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovalDate",
                table: "Friends",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequestFlag",
                table: "Friends",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalDate",
                table: "Friends");

            migrationBuilder.DropColumn(
                name: "RequestFlag",
                table: "Friends");

            migrationBuilder.AddColumn<DateTime>(
                name: "BecameFriendsTime",
                table: "Friends",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FriendRequestFlag",
                table: "Friends",
                nullable: false,
                defaultValue: 0);
        }
    }
}
