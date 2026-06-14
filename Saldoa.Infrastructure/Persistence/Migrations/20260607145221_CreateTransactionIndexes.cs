using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saldoa.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateTransactionIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_transactions_user",
                schema: "app",
                table: "transactions");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_installment_group_id",
                schema: "app",
                table: "transactions",
                column: "installment_group_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_user_id_paid_or_received_at",
                schema: "app",
                table: "transactions",
                columns: new[] { "user_id", "paid_or_received_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_transactions_installment_group_id",
                schema: "app",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_transactions_user_id_paid_or_received_at",
                schema: "app",
                table: "transactions");

            migrationBuilder.CreateIndex(
                name: "ix_transactions_user",
                schema: "app",
                table: "transactions",
                column: "user_id");
        }
    }
}
