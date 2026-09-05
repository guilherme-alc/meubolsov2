namespace Saldoa.API.Endpoints.CategoryBudgets;

internal static class CategoryBudgetsEndpoints
{
    internal static void MapCategoryBudgetEndpoints(this IEndpointRouteBuilder app)
    {
        var categoryBudgetsGroup = app.MapGroup("/category-budgets")
            .WithTags("Category Budgets");

        CreateCategoryBudgetEndpoint.Map(categoryBudgetsGroup);
        ListCategoryBudgetsEndpoint.Map(categoryBudgetsGroup);
        DeleteCategoryBudgetEndpoint.Map(categoryBudgetsGroup);
        GetCategoryBudgetByIdEndpoint.Map(categoryBudgetsGroup);
        UpdateCategoryBudgetEndpoint.Map(categoryBudgetsGroup);
        GetCategoryBudgetsByCategoryEndpoint.Map(categoryBudgetsGroup);
    }
}