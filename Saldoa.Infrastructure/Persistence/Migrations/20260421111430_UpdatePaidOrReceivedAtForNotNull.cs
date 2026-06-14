using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saldoa.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePaidOrReceivedAtForNotNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "paid_or_received_at",
                schema: "app",
                table: "transactions",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "paid_or_received_at",
                schema: "app",
                table: "transactions",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }
    }
}
