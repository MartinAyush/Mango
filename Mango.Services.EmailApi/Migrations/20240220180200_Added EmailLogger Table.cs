﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mango.Services.EmailApi.Migrations
{
    /// <inheritdoc />
    public partial class AddedEmailLoggerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailLoggers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<int>(type: "int", nullable: false),
                    EmailSent = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailLoggers", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailLoggers");
        }
    }
}
