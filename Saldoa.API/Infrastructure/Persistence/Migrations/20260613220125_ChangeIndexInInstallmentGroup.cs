using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saldoa.API.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeIndexInInstallmentGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_transactions_installment_group_id",
                schema: "app",
                table: "transactions");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_installment_group_id",
                schema: "app",
                table: "transactions",
                column: "installment_group_id",
                filter: "installment_group_id IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_transactions_installment_group_id",
                schema: "app",
                table: "transactions");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_installment_group_id",
                schema: "app",
                table: "transactions",
                column: "installment_group_id");
        }
    }
}
