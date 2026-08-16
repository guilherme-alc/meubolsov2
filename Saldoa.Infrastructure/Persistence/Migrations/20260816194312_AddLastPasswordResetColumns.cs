using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saldoa.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLastPasswordResetColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "last_password_reset_at",
                schema: "auth",
                table: "users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_password_reset_email_sent_at",
                schema: "auth",
                table: "users",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "last_password_reset_at",
                schema: "auth",
                table: "users");

            migrationBuilder.DropColumn(
                name: "last_password_reset_email_sent_at",
                schema: "auth",
                table: "users");
        }
    }
}
