using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saldoa.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateInstallmentIntoTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "installment_group_id",
                schema: "app",
                table: "transactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "installment_number",
                schema: "app",
                table: "transactions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "installment_total",
                schema: "app",
                table: "transactions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "installment_group_id",
                schema: "app",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "installment_number",
                schema: "app",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "installment_total",
                schema: "app",
                table: "transactions");
        }
    }
}
