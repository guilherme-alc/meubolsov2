using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saldoa.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLastConfirmationEmailSentAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "full_name",
                schema: "auth",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "customer_id",
                schema: "auth",
                table: "users",
                newName: "last_name");

            migrationBuilder.AddColumn<string>(
                name: "first_name",
                schema: "auth",
                table: "users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "last_confirmation_email_sent_at",
                schema: "auth",
                table: "users",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "first_name",
                schema: "auth",
                table: "users");

            migrationBuilder.DropColumn(
                name: "last_confirmation_email_sent_at",
                schema: "auth",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "last_name",
                schema: "auth",
                table: "users",
                newName: "customer_id");

            migrationBuilder.AddColumn<string>(
                name: "full_name",
                schema: "auth",
                table: "users",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);
        }
    }
}
