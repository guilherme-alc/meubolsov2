namespace MeuBolso.API.Endpoints.Categories;

public static class CategoriesEndpoints
{
    public static void MapCategoriesEndpoint(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/categories")
            .RequireAuthorization()
            .WithTags("Categories");

        CreateCategoryEndpoint.Map(group);
    }
}