﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetCore.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddingUserTable_UserEmailColumn_UniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_User_UserEmail",
                table: "User");

            migrationBuilder.CreateIndex(
                name: "IX_User_UserEmail",
                table: "User",
                column: "UserEmail",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_User_UserEmail",
                table: "User");

            migrationBuilder.CreateIndex(
                name: "IX_User_UserEmail",
                table: "User",
                column: "UserEmail");
        }
    }
}
