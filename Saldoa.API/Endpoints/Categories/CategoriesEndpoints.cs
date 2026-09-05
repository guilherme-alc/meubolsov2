namespace Saldoa.API.Endpoints.Categories;

internal static class CategoriesEndpoints
{
    internal static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var categoriesGroup = app.MapGroup("/categories")
            .WithTags("Categories");

        CreateCategoryEndpoint.Map(categoriesGroup);
        UpdateCategoryEndpoint.Map(categoriesGroup);
        GetCategoryByIdEndpoint.Map(categoriesGroup);
        DeleteCategoryEndpoint.Map(categoriesGroup);
        ListCategoriesEndpoint.Map(categoriesGroup);
    }
}