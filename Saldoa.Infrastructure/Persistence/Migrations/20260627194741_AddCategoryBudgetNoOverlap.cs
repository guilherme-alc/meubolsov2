using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saldoa.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryBudgetNoOverlap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
            CREATE EXTENSION IF NOT EXISTS btree_gist;
            """);

            migrationBuilder.Sql("""
            ALTER TABLE app.category_budgets
            ADD CONSTRAINT ex_category_budgets_no_overlap
            EXCLUDE USING gist
            (
                user_id WITH =,
                category_id WITH =,
                daterange(period_start, period_end, '[]') WITH &&
            );
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
            ALTER TABLE app.category_budgets
            DROP CONSTRAINT IF EXISTS ex_category_budgets_no_overlap;
            """);
        }
    }
}
