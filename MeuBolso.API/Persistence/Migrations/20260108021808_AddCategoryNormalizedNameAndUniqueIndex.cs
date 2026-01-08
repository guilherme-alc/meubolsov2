using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeuBolso.API.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryNormalizedNameAndUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_categories_user_name",
                schema: "app",
                table: "categories");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                schema: "app",
                table: "categories",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ux_categories_user_normalized_name",
                schema: "app",
                table: "categories",
                columns: new[] { "user_id", "NormalizedName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_categories_user_normalized_name",
                schema: "app",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "NormalizedName",
                schema: "app",
                table: "categories");

            migrationBuilder.CreateIndex(
                name: "ux_categories_user_name",
                schema: "app",
                table: "categories",
                columns: new[] { "user_id", "name" },
                unique: true);
        }
    }
}
