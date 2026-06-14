using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saldoa.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AlterDeleteCategoryBudgetForCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_category_budgets_category",
                schema: "app",
                table: "category_budgets");

            migrationBuilder.AddForeignKey(
                name: "fk_category_budgets_category",
                schema: "app",
                table: "category_budgets",
                column: "category_id",
                principalSchema: "app",
                principalTable: "categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_category_budgets_category",
                schema: "app",
                table: "category_budgets");

            migrationBuilder.AddForeignKey(
                name: "fk_category_budgets_category",
                schema: "app",
                table: "category_budgets",
                column: "category_id",
                principalSchema: "app",
                principalTable: "categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
