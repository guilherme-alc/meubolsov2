using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeuBolso.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexesAndRenameColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_categories_Users_user_id",
                schema: "app",
                table: "categories");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_Users_user_id",
                schema: "app",
                table: "transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_categories_CategoryId",
                schema: "app",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_categories_user_id",
                schema: "app",
                table: "categories");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "app",
                table: "transactions",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                schema: "app",
                table: "transactions",
                newName: "category_id");

            migrationBuilder.RenameColumn(
                name: "payment_or_received_at",
                schema: "app",
                table: "transactions",
                newName: "paid_or_received_at");

            migrationBuilder.RenameIndex(
                name: "IX_transactions_user_id",
                schema: "app",
                table: "transactions",
                newName: "ix_transactions_user");

            migrationBuilder.RenameIndex(
                name: "IX_transactions_CategoryId",
                schema: "app",
                table: "transactions",
                newName: "IX_transactions_category_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "app",
                table: "categories",
                newName: "id");

            migrationBuilder.CreateIndex(
                name: "ux_categories_user_name",
                schema: "app",
                table: "categories",
                columns: new[] { "user_id", "name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_categories_user",
                schema: "app",
                table: "categories",
                column: "user_id",
                principalSchema: "auth",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_transactions_category",
                schema: "app",
                table: "transactions",
                column: "category_id",
                principalSchema: "app",
                principalTable: "categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_transactions_user",
                schema: "app",
                table: "transactions",
                column: "user_id",
                principalSchema: "auth",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_categories_user",
                schema: "app",
                table: "categories");

            migrationBuilder.DropForeignKey(
                name: "fk_transactions_category",
                schema: "app",
                table: "transactions");

            migrationBuilder.DropForeignKey(
                name: "fk_transactions_user",
                schema: "app",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "ux_categories_user_name",
                schema: "app",
                table: "categories");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "app",
                table: "transactions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "category_id",
                schema: "app",
                table: "transactions",
                newName: "CategoryId");

            migrationBuilder.RenameColumn(
                name: "paid_or_received_at",
                schema: "app",
                table: "transactions",
                newName: "payment_or_received_at");

            migrationBuilder.RenameIndex(
                name: "ix_transactions_user",
                schema: "app",
                table: "transactions",
                newName: "IX_transactions_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_transactions_category_id",
                schema: "app",
                table: "transactions",
                newName: "IX_transactions_CategoryId");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "app",
                table: "categories",
                newName: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_categories_user_id",
                schema: "app",
                table: "categories",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_categories_Users_user_id",
                schema: "app",
                table: "categories",
                column: "user_id",
                principalSchema: "auth",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_Users_user_id",
                schema: "app",
                table: "transactions",
                column: "user_id",
                principalSchema: "auth",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_categories_CategoryId",
                schema: "app",
                table: "transactions",
                column: "CategoryId",
                principalSchema: "app",
                principalTable: "categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
