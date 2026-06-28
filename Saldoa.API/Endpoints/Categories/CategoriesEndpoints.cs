namespace Saldoa.API.Endpoints.Categories;

public static class CategoriesEndpoints
{
    public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
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